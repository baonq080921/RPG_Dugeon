using System;
using System.Collections.Generic;
using Base;
using UnityEngine;

public class InventoryBase : MonoBehaviour 
{

    [SerializeField] private int _inventoryMaxSize = 12;
    [field:SerializeField] public List<ItemInventory> itemInventoriesList {get; private set;}
    public bool CanAddToIventory() => itemInventoriesList.Count < _inventoryMaxSize;


    protected virtual void Awake(){}

    #region Inventotry
    public void AddToInventory(ItemInventory itemToAdd)
    {
        ItemInventory item = FindItem(itemToAdd);
        if(item != null && item.CanAddToStack())
            item.AddStack();
        else
            itemInventoriesList.Add(itemToAdd);

        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }

    public void ClearFromInventory(ItemInventory item)
    {
        itemInventoriesList.Remove(FindItem(item));
        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }

    public void DropItem(ItemInventory item)
    {
        ItemInventory found = FindItem(item);
        if (found == null) return;
        if (found.stackSize > 1)
            found.RemoveStackSize();
        else
            itemInventoriesList.Remove(found);
        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }

    [ContextMenu("Clear all the Inventory")]
    public void ClearAllInventory() => itemInventoriesList.Clear();
    #endregion

    public ItemInventory FindItem(ItemInventory itemAdd)
    {
        return itemInventoriesList.Find(item => item.itemData == itemAdd.itemData);
    }

    /// <summary>
    /// Returns the total stack count of all items matching the given type.
    /// </summary>
    public int GetCountByType(ItemTypes itemType)
    {
        int total = 0;
        foreach (ItemInventory item in itemInventoriesList)
        {
            if (item.itemData.ItemType == itemType)
                total += item.stackSize;
        }
        return total;
    }

    /// <summary>
    /// Removes the specified amount of items matching the given type, consuming stacks as needed.
    /// </summary>
    public void ConsumeByType(ItemTypes itemType, int amount)
    {
        int remaining = amount;
        for (int i = itemInventoriesList.Count - 1; i >= 0 && remaining > 0; i--)
        {
            ItemInventory item = itemInventoriesList[i];
            if (item.itemData.ItemType != itemType) continue;

            int toConsume = Mathf.Min(remaining, item.stackSize);
            remaining -= toConsume;
            for (int j = 0; j < toConsume; j++)
            {
                if (item.stackSize > 1)
                    item.RemoveStackSize();
                else
                {
                    itemInventoriesList.RemoveAt(i);
                    break;
                }
            }
        }
        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }
}