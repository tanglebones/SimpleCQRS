using System;

namespace SimpleCQRS
{
    public class InventoryItemCheckedInEvent : IEvent
    {
        public Guid Id;
        public readonly int Count;
 
        public InventoryItemCheckedInEvent(Guid id, int count) {
            Id = id;
            Count = count;
        }
        public int Revision { get; set; }
    }
}