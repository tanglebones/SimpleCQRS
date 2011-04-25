using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCQRS
{
    public class EventStore : IEventStore
    {
        private readonly IEventPublisher _publisher;

        private struct EventDescriptor
        {
            
            public readonly IEvent EventData;
            public readonly Guid Id;
            public readonly int Revision;

            public EventDescriptor(Guid id, IEvent eventData, int revision)
            {
                EventData = eventData;
                Revision = revision;
                Id = id;
            }
        }

        public EventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        private readonly Dictionary<Guid, List<EventDescriptor>> _current = new Dictionary<Guid, List<EventDescriptor>>(); 
        
        public void SaveEvents(Guid aggregateId, IEnumerable<IEvent> events, int expectedRevision)
        {
            List<EventDescriptor> eventDescriptors;
            if(!_current.TryGetValue(aggregateId, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                _current.Add(aggregateId,eventDescriptors);
            }
            else if(eventDescriptors[eventDescriptors.Count - 1].Revision != expectedRevision && expectedRevision != -1)
            {
                throw new ConcurrencyException();
            }
            var i = expectedRevision;
            foreach (var @event in events)
            {
                i++;
                @event.Revision = i;
                eventDescriptors.Add(new EventDescriptor(aggregateId,@event,i));
                _publisher.Publish(@event);
            }
        }

        public  IEnumerable<IEvent> GetEventsForAggregate(Guid aggregateId)
        {
            List<EventDescriptor> eventDescriptors;
            if (!_current.TryGetValue(aggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }
            return eventDescriptors.Select(desc => desc.EventData).ToList();
        }
    }

    public class ConcurrencyException : Exception
    {
    }
}