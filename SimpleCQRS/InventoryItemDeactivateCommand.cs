using System;

namespace SimpleCQRS
{
    public class InventoryItemDeactivateCommand : ICommand {
        public readonly Guid InventoryItemId;
        public readonly int OriginalVersion;

        public InventoryItemDeactivateCommand(Guid inventoryItemId, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            OriginalVersion = originalVersion;
        }
    }
}