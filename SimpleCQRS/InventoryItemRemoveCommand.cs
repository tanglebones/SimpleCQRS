using System;

namespace SimpleCQRS
{
    public class InventoryItemRemoveCommand : ICommand {
        public Guid InventoryItemId;
        public readonly int Count;
        public readonly int OriginalVersion;

        public InventoryItemRemoveCommand(Guid inventoryItemId, int count, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            Count = count;
            OriginalVersion = originalVersion;
        }
    }
}