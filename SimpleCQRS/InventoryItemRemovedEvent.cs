using System;

namespace SimpleCQRS
{
    public class InventoryItemRemovedEvent : IEvent
    {
        public Guid Id;
        public readonly int Count;
 
        public InventoryItemRemovedEvent(Guid id, int count) {
            Id = id;
            Count = count;
        }
        public int Revision { get; set; }
    }
}