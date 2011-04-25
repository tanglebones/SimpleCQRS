using System;

namespace SimpleCQRS
{
    public class InventoryItemRenameCommand : ICommand {
        public readonly Guid InventoryItemId;
        public readonly string NewName;
        public readonly int OriginalVersion;

        public InventoryItemRenameCommand(Guid inventoryItemId, string newName, int originalVersion)
        {
            InventoryItemId = inventoryItemId;
            NewName = newName;
            OriginalVersion = originalVersion;
        }
    }
}