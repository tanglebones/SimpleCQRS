namespace SimpleCQRS
{
    public class InventoryItemCommandHandlers
    {
        private readonly IRepository<InventoryItemAggregateRoot> _repository;
        public InventoryItemCommandHandlers(IRepository<InventoryItemAggregateRoot> repository)
        {
            _repository = repository;
        }
        public void Handle(InventoryItemCreateCommand message)
        {
            var item = new InventoryItemAggregateRoot(message.InventoryItemId, message.Name);
            _repository.Save(item, -1);
        }
        public void Handle(InventoryItemDeactivateCommand message)
        {
            var item = _repository.GetById(message.InventoryItemId);
            item.Deactivate();
            _repository.Save(item, message.OriginalVersion);
        }
        public void Handle(InventoryItemRemoveCommand message)
        {
            var item = _repository.GetById(message.InventoryItemId);
            item.Remove(message.Count);
            _repository.Save(item, message.OriginalVersion);
        }
        public void Handle(InventoryItemCheckInCommand message)
        {
            var item = _repository.GetById(message.InventoryItemId);
            item.CheckIn(message.Count);
            _repository.Save(item, message.OriginalVersion);
        }
        public void Handle(InventoryItemRenameCommand message)
        {
            var item = _repository.GetById(message.InventoryItemId);
            item.ChangeName(message.NewName);
            _repository.Save(item, message.OriginalVersion);
        }
    }
}