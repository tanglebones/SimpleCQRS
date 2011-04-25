using System;

namespace SimpleCQRS
{
    public class InventoryItemRenamedEvent : IEvent
    {
        public readonly Guid Id;
        public readonly string NewName;
 
        public InventoryItemRenamedEvent(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
        public int Revision { get; set; }
    }
}