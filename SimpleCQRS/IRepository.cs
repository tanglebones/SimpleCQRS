using System;

namespace SimpleCQRS
{
    public interface IRepository<out T> where T : new()
    {
        void Save(IAggregateRoot aggregate, int expectedRevision);
        T GetById(Guid id);
    }
}