using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;
using CQRSlite.Messages;

namespace CQRSlite.Bus
{
    public class InProcessBus : ICommandSender, IEventPublisher, IHandlerRegistrar
    {
        private readonly Dictionary<Type, List<Func<IMessage, Task>>> _routes = new Dictionary<Type, List<Func<IMessage,Task>>>();

        public void RegisterHandler<T>(Func<T, Task> handler) where T : IMessage
        {
            List<Func<IMessage, Task>> handlers;
            if(!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Func<IMessage, Task>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add((x => handler((T)x)));
        }

        public async Task SendAsync<T>(T command) where T : ICommand
        {
            List<Func<IMessage, Task>> handlers; 
            if (_routes.TryGetValue(command.GetType(), out handlers))
            {
                if (handlers.Count != 1)
                {
                    var message = string.Format("Cannot send command {0} to more than one handler", command.GetType());
                    throw new InvalidOperationException(message);
                }
                try
                {
                    await handlers[0](command);
                }
                catch (AggregateException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new CommandHandlerFailedException(command, e);
                }
            }
            else
            {
                var message = string.Format("No handler registered for command {0}", command.GetType());
                throw new InvalidOperationException(message);
            }
        }

        public async Task PublishAsync<T>(T @event) where T : IEvent
        {
            List<Exception> exceptions = new List<Exception>();
            List<Func<IMessage, Task>> handlers;
            var eventType = @event.GetType();
            if (!_routes.TryGetValue(eventType, out handlers)) return;
            foreach (var handler in handlers)
            {
                try
                {
                    await handler.Invoke(@event);
                }
                catch (Exception e)
                {
                    exceptions.Add( new EventHandlerFailedException(@event, e));
                }
            }
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
