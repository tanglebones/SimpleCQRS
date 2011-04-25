using System;

namespace SimpleCQRS
{
    public class Repository<T> : IRepository<T> where T: IAggregateRoot, new() //shortcut you can do as you see fit with new()
    {
        private readonly IEventStore _storage;

        public Repository(IEventStore storage)
        {
            _storage = storage;
        }

        public void Save(IAggregateRoot aggregate, int expectedRevision)
        {
            _storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedRevision);
        }

        public T GetById(Guid id)
        {
            var obj = new T();//lots of ways to do this
            var e = _storage.GetEventsForAggregate(id);
            obj.LoadsFromHistory(e);
            return obj;
        }
    }
}