using UnityEngine;


public class ItemCraft
{
    [field:SerializeField] public ItemCraftData itemCraftData{get; private set;}
    public ItemCraft(ItemCraftData itemCraftData)
    {
        this.itemCraftData = itemCraftData;
    }
}