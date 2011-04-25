using System;
using System.Collections.Generic;

namespace SimpleCQRS
{
    public interface IEventStore
    {
        void SaveEvents(Guid aggregateId, IEnumerable<IEvent> events, int expectedRevision);
        IEnumerable<IEvent> GetEventsForAggregate(Guid aggregateId);
    }
}