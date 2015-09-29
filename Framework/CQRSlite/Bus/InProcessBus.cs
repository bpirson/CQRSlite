using System;
using System.Collections.Generic;
using System.Linq;
using CQRSlite.Commands;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;
using CQRSlite.Messages;

namespace CQRSlite.Bus
{
    public class InProcessBus : ICommandSender, IEventPublisher, IHandlerRegistrar
    {
        private readonly Dictionary<Type, List<Action<IMessage>>> _routes = new Dictionary<Type, List<Action<IMessage>>>();

        public void RegisterHandler<T>(Action<T> handler) where T : IMessage
        {
            List<Action<IMessage>> handlers;
            if(!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<IMessage>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add((x => handler((T)x)));
        }

        public void Send<T>(T command) where T : ICommand
        {
            List<Action<IMessage>> handlers; 
            if (_routes.TryGetValue(command.GetType(), out handlers))
            {
                if (handlers.Count != 1)
                {
                    var message = string.Format("Cannot send command {0} to more than one handler", command.GetType());
                    throw new InvalidOperationException(message);
                }
                try
                {
                    handlers[0](command);
                }
                catch (AggregateException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new CommandHandlerFailedException(command, handlers[0], e);
                }
            }
            else
            {
                var message = string.Format("No handler registered for command {0}", command.GetType());
                throw new InvalidOperationException(message);
            }
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            List<Exception> exceptions = new List<Exception>();
            List<Action<IMessage>> handlers;
            var eventType = @event.GetType();
            if (!_routes.TryGetValue(eventType, out handlers)) return;
            foreach (var handler in handlers)
            {
                try
                {
                    handler.Invoke(@event);
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
