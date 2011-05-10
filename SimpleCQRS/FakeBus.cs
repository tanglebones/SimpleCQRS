using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleCQRS
{
    public class FakeBus : IBus
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
            handlers.Add(DelegateAdjuster.CastArgument<IMessage, T>(x => handler(x)));
        }

        public void Send<T>(T command) where T: ICommand
        {
            List<Action<IMessage>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                throw new InvalidOperationException("no handler registered");
            }
            if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one handler");
            handlers[0](command);
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            List<Action<IMessage>> handlers; 
            if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;
            foreach(var handler in handlers)
            {
                //dispatch on thread pool for added awesomeness
                var handler1 = handler;
                ThreadPool.QueueUserWorkItem(x => handler1(@event));
            }
        }
    }
}
