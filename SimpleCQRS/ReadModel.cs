using System;
using System.Collections.Generic;

namespace SimpleCQRS
{
    public interface IReadModelFacade
    {
        IEnumerable<InventoryItemListDto> GetInventoryItems();
        InventoryItemDetailsDto GetInventoryItemDetails(Guid id);
    }

    public class InventoryItemDetailsDto
    {
        public int CurrentCount;
        public Guid Id;
        public string Name;
        public int Version;

        public InventoryItemDetailsDto(Guid id, string name, int currentCount, int version)
        {
            Id = id;
            Name = name;
            CurrentCount = currentCount;
            Version = version;
        }
    }

    public class InventoryItemListDto
    {
        public Guid Id;
        public string Name;

        public InventoryItemListDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class InventoryListView
        : IHandle<InventoryItemCreatedEvent>, IHandle<InventoryItemRenamedEvent>,
          IHandle<InventoryItemDeactivatedEvent>
    {
        public void Handle(InventoryItemCreatedEvent message)
        {
            SimpleInMemoryDatabase.List.Add(new InventoryItemListDto(message.Id, message.Name));
        }

        public void Handle(InventoryItemDeactivatedEvent message)
        {
            SimpleInMemoryDatabase.List.RemoveAll(x => x.Id == message.Id);
        }

        public void Handle(InventoryItemRenamedEvent message)
        {
            var item = SimpleInMemoryDatabase.List.Find(x => x.Id == message.Id);
            item.Name = message.NewName;
        }
    }

    public class InventoryItemDetailView
        : IHandle<InventoryItemCreatedEvent>, IHandle<InventoryItemDeactivatedEvent>,
          IHandle<InventoryItemRenamedEvent>, IHandle<InventoryItemRemovedEvent>,
          IHandle<InventoryItemCheckedInEvent>
    {
        public void Handle(InventoryItemCheckedInEvent message)
        {
            var d = GetDetailsItem(message.Id);
            d.CurrentCount += message.Count;
            d.Version = message.Revision;
        }

        public void Handle(InventoryItemCreatedEvent message)
        {
            SimpleInMemoryDatabase.Details.Add(message.Id, new InventoryItemDetailsDto(message.Id, message.Name, 0, 0));
        }

        public void Handle(InventoryItemDeactivatedEvent message)
        {
            SimpleInMemoryDatabase.Details.Remove(message.Id);
        }

        public void Handle(InventoryItemRemovedEvent message)
        {
            var d = GetDetailsItem(message.Id);
            d.CurrentCount -= message.Count;
            d.Version = message.Revision;
        }

        public void Handle(InventoryItemRenamedEvent message)
        {
            var d = GetDetailsItem(message.Id);
            d.Name = message.NewName;
            d.Version = message.Revision;
        }

        private static InventoryItemDetailsDto GetDetailsItem(Guid id)
        {
            InventoryItemDetailsDto d;
            if (!SimpleInMemoryDatabase.Details.TryGetValue(id, out d))
            {
                throw new InvalidOperationException("did not find the original inventory this shouldnt happen");
            }
            return d;
        }
    }

    public class ReadModelFacade : IReadModelFacade
    {
        public IEnumerable<InventoryItemListDto> GetInventoryItems()
        {
            return SimpleInMemoryDatabase.List;
        }

        public InventoryItemDetailsDto GetInventoryItemDetails(Guid id)
        {
            return SimpleInMemoryDatabase.Details[id];
        }
    }

    public static class SimpleInMemoryDatabase
    {
        public static readonly Dictionary<Guid, InventoryItemDetailsDto> Details =
            new Dictionary<Guid, InventoryItemDetailsDto>();

        public static readonly List<InventoryItemListDto> List = new List<InventoryItemListDto>();
    }
}