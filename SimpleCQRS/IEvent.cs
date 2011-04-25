namespace SimpleCQRS
{
    public interface IEvent : IMessage
    {
        int Revision { get; set; }
    }
}