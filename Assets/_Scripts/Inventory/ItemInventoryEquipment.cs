namespace Inventory
{
    [System.Serializable]
    public class ItemInventoryEquipment
    {
        public EquipSlotType slotType;
        public ItemInventory equipItem;
        public bool HasItem() => equipItem != null && equipItem.itemData != null;
    }
}
