using System;
using System.Collections.Generic;
using Base;
using DG.Tweening;
using player;
using TMPro;
using UnityEngine;

public class PlayerInventory : InventoryBase {

    private EntityStat _playerStats;
    private Player _player;
    [field:SerializeField] public List<ItemInventoryEquipment> equipList {get; private set;}
    private EventBinding<EquipEvent> _eventEquipBinding;
    private EventBinding<PlayerDiedEvent> _eventDiedBinding;

    protected override void Awake()
    {
        base.Awake();
        _playerStats = GetComponent<EntityStat>();
        _player = GetComponent<Player>();
    }

    void OnEnable()
    {
        _eventEquipBinding = new EventBinding<EquipEvent>(TryToEquipEvent);
        EventBus<EquipEvent>.Register(_eventEquipBinding);
        _eventDiedBinding = new EventBinding<PlayerDiedEvent>(DropAllItem);
        EventBus<PlayerDiedEvent>.Register(_eventDiedBinding);

    }

    void OnDisable()
    {
        EventBus<EquipEvent>.Deregister(_eventEquipBinding);
        EventBus<PlayerDiedEvent>.Deregister(_eventDiedBinding);
    }


    public void TryToEquipEvent(EquipEvent equipEvent) => TryEquipItem(equipEvent.itemInventory);


    private void TryEquipItem(ItemInventory item)
    {
        if (item == null || item.itemData == null) return;

        if (item.itemData.EquipSlot == EquipSlotType.None)
        {
            EventBus<AlertNotiEvent>.Raise(new AlertNotiEvent(GameMessages.Alert("Item Cannot Be Equipped")));
            return;
        }

        ItemInventory itemInventory = FindItem(item);
        if (itemInventory == null) return;

        ItemInventoryEquipment targetSlot = equipList.Find(slot => slot.slotType == item.itemData.EquipSlot);
        if (targetSlot == null || targetSlot.HasItem())
        {
            EventBus<AlertNotiEvent>.Raise(new AlertNotiEvent(GameMessages.Alert("Weapon Already In Slot")));
            return;
        }
        EquipItem(itemInventory, targetSlot);
    }


    private void EquipItem(ItemInventory item, ItemInventoryEquipment slot)
    {
        if (item.stackSize > 1)
        {
            item.RemoveStackSize();
            slot.equipItem = new ItemInventory(item.itemData);
        }
        else
        {
            slot.equipItem = item;
            ClearFromInventory(item);
        }
        slot.equipItem.AddModifiers(_playerStats);
        slot.equipItem.AddItemEffect(_player);
        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }

    /// <summary>
    /// Removes the item from the equipment slot and returns it to the inventory.
    /// If the inventory is full and cannot stack, the item is discarded.
    /// </summary>
    /// <param name="slot">The equipment slot to unequip.</param>
    public void UnequipItem(ItemInventoryEquipment slot)
    {
        if (!slot.HasItem()) return;

        ItemInventory item = slot.equipItem;
        slot.equipItem.RemoveModifiers(_playerStats);
        item.RemoveItemEffect(_player);
        slot.equipItem = null;
        ItemInventory stackable = FindItem(item);
        bool canStack = stackable != null && stackable.CanAddToStack();
        if (CanAddToIventory() || canStack)
            AddToInventory(item);
        else
            EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }

    /// <summary>
    /// Removes the item from the equipment slot and discards it without returning it to inventory.
    /// </summary>
    /// <param name="slot">The equipment slot to clear.</param>
    public void DropEquippedItem(ItemInventoryEquipment slot)
    {
        if (!slot.HasItem()) return;
        slot.equipItem.RemoveModifiers(_playerStats);
        slot.equipItem.RemoveItemEffect(_player);
        slot.equipItem = null;
        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }

    public void DropAllItem()
    {
        foreach (var item in itemInventoriesList)
            for (int i = 0; i < item.stackSize; i++)
                SpawnDroppedItem(item.itemData);
        itemInventoriesList.Clear();

        foreach (var slot in equipList)
        {
            if (!slot.HasItem()) continue;
            SpawnDroppedItem(slot.equipItem.itemData);
            slot.equipItem.RemoveModifiers(_playerStats);
            slot.equipItem.RemoveItemEffect(_player);
            slot.equipItem = null;
        }

        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }

    private void SpawnDroppedItem(ItemData itemData) =>
        ServiceLocator.Get<ItemPickablePool>()?.Spawn(itemData, transform.position);
}