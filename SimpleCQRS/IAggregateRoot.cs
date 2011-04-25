using System;
using System.Collections.Generic;

namespace SimpleCQRS
{
    public interface IAggregateRoot
    {
        Guid Id { get; }
        int Revision { get; }

        IEnumerable<IEvent> GetUncommittedChanges();
        void MarkChangesAsCommitted();
        void LoadsFromHistory(IEnumerable<IEvent> history);
    }
}