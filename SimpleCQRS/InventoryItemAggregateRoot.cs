using System;
using System.Collections.Generic;

namespace SimpleCQRS
{
    public class InventoryItemAggregateRoot : IAggregateRoot
    {
        private readonly IList<IEvent> _changes = new List<IEvent>();
        private bool _activated;

        public InventoryItemAggregateRoot()
        {
            // used to create in repository ... many ways to avoid this, eg making private constructor
        }

        public InventoryItemAggregateRoot(Guid id, string name)
        {
            ApplyChange(new InventoryItemCreatedEvent(id, name));
        }

        public Guid Id { get; private set; }
        public int Revision { get; private set; }

        public IEnumerable<IEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history) ApplyChange(e, false);
        }

        private void Apply(InventoryItemCreatedEvent e)
        {
            Id = e.Id;
            _activated = true;
        }

        private void Apply(InventoryItemDeactivatedEvent e)
        {
            _activated = false;
        }

        private void Apply(IEvent e)
        {
        }

        public void ChangeName(string newName)
        {
            if (string.IsNullOrEmpty(newName)) throw new ArgumentException("newName");
            ApplyChange(new InventoryItemRenamedEvent(Id, newName));
        }

        public void Remove(int count)
        {
            if (count <= 0) throw new InvalidOperationException("cant remove negative count from inventory");
            ApplyChange(new InventoryItemRemovedEvent(Id, count));
        }

        public void CheckIn(int count)
        {
            if (count <= 0) throw new InvalidOperationException("must have a count greater than 0 to add to inventory");
            ApplyChange(new InventoryItemCheckedInEvent(Id, count));
        }

        public void Deactivate()
        {
            if (!_activated) throw new InvalidOperationException("already deactivated");
            ApplyChange(new InventoryItemDeactivatedEvent(Id));
        }

        private void ApplyChange(IEvent @event, bool isNew = true)
        {
            Apply((dynamic) @event);
            if (isNew) _changes.Add(@event);
        }
    }
}