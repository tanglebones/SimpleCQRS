using System;

namespace SimpleCQRS
{
    public class InventoryItemCreateCommand : ICommand {
        public readonly Guid InventoryItemId;
        public readonly string Name;
	    
        public InventoryItemCreateCommand(Guid inventoryItemId, string name)
        {
            InventoryItemId = inventoryItemId;
            Name = name;
        }
    }
}