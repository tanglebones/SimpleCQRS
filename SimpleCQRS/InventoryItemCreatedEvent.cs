using System;

namespace SimpleCQRS
{
    public class InventoryItemCreatedEvent : IEvent {
        public readonly Guid Id;
        public readonly string Name;
        public InventoryItemCreatedEvent(Guid id, string name) {
            Id = id;
            Name = name;
        }
        public int Revision { get; set; }
    }
}