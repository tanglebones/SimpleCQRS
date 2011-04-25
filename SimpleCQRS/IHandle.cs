namespace SimpleCQRS
{
    public interface IHandle<in T>
    {
        void Handle(T message);
    }
}