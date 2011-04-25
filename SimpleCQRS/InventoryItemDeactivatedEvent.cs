using System;

namespace SimpleCQRS
{
    public class InventoryItemDeactivatedEvent : IEvent {
        public readonly Guid Id;

        public InventoryItemDeactivatedEvent(Guid id)
        {
            Id = id;
        }

        public int Revision { get; set; }
    }
}