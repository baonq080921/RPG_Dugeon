using Base;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Raised when an item should be dropped into the world at <see cref="position"/>.
    /// The Pool layer listens and spawns the world pickup, so Inventory never references the Pool package.
    /// </summary>
    public struct DropItemRequestedEvent : IEvent
    {
        public ItemData itemData;
        public Vector3 position;
        public DropItemRequestedEvent(ItemData itemData, Vector3 position)
        {
            this.itemData = itemData;
            this.position = position;
        }
    }

    /// <summary>Raised when a craft item is selected, carrying its data so the details panel can display it.</summary>
    public struct CraftGetInfoEvent : IEvent
    {
        public ItemCraftData itemCraftData;
        public CraftGetInfoEvent(ItemCraftData itemCraftData)
        {
            this.itemCraftData = itemCraftData;
        }
    }

    /// <summary>Raised when a shop item is selected, carrying its data so the details panel can display it.</summary>
    public struct StoreItemGetInfoEvent : IEvent
    {
        public ItemData itemData;
        public StoreItemGetInfoEvent(ItemData itemData)
        {
            this.itemData = itemData;
        }
    }

    /// <summary>Raised when the player requests to equip an inventory item.</summary>
    public struct EquipEvent : IEvent
    {
        public ItemInventory itemInventory;
        public EquipEvent(ItemInventory itemInventory)
        {
            this.itemInventory = itemInventory;
        }
    }
}
