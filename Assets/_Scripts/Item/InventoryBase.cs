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
}