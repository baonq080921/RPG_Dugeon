[System.Serializable]
public class ItemInventoryEquipment 
{
    public ItemTypes slotType;
    public ItemInventory equipItem;
    public bool HasItem() => equipItem != null && equipItem.itemData != null;
}