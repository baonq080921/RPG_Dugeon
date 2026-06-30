using Base;

/// <summary>Raised when a craft category slot is tapped, carrying the item's craft data for the detail panel.</summary>
public struct CraftGetInfoEvent : IEvent
{
    public ItemCraftData itemCraftData;
    public CraftGetInfoEvent(ItemCraftData itemCraftData)
    {
        this.itemCraftData = itemCraftData;
    }
}

/// <summary>Raised when a shop item slot is tapped, carrying the item data for the detail panel.</summary>
public struct StoreItemGetInfoEvent : IEvent
{
    public ItemData itemData;
    public StoreItemGetInfoEvent(ItemData itemData)
    {
        this.itemData = itemData;
    }
}

/// <summary>Raised when the player chooses to equip an inventory item.</summary>
public struct EquipEvent : IEvent
{
    public ItemInventory itemInventory;
    public EquipEvent(ItemInventory itemInventory)
    {
        this.itemInventory = itemInventory;
    }
}
