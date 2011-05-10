using System;

namespace SimpleCQRS
{
    public interface IBus: ICommandSender, IEventPublisher
    {
        void RegisterHandler<T>(Action<T> handler) where T : IMessage;
    }
}