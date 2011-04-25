using System;

namespace SimpleCQRS
{
    public class InventoryItemCheckInCommand : ICommand {
        public Guid InventoryItemId;
        public readonly int Count;
        public readonly int OriginalVersion;

        public InventoryItemCheckInCommand(Guid inventoryItemId, int count, int originalVersion) {
            InventoryItemId = inventoryItemId;
            Count = count;
            OriginalVersion = originalVersion;
        }
    }
}