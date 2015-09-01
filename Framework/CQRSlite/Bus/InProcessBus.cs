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
                if (handlers.Count != 1) throw new InvalidOperationException("Cannot send to more than one handler");
                handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("No handler registered");
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
                    handler(@event);
                }
                catch (Exception e)
                {
                    exceptions.Add( new EventHandlerFailedException(@event, handler,e));
                }
            }
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
