using System.Collections.Generic;
using Base;
using UnityEngine;

public class PlayerInventory : InventoryBase {
    
    private EntityStat _playerStats;
    [field:SerializeField] public List<ItemInventoryEquipment> equipList {get; private set;}
    private EventBinding<EquipEvent> eventEquipBinding;

    protected override void Awake()
    {
        base.Awake();
        _playerStats = GetComponent<EntityStat>();
    }

    void OnEnable()
    {
        eventEquipBinding = new EventBinding<EquipEvent>(TryToEquipEvent);
        EventBus<EquipEvent>.Register(eventEquipBinding);
    }

    void OnDisable()
    {
        EventBus<EquipEvent>.Deregister(eventEquipBinding);
    }


    public void TryToEquipEvent(EquipEvent equipEvent) => TryEquipItem(equipEvent.itemInventory);


    private void TryEquipItem(ItemInventory item)
    {
        if (item == null || item.itemData == null) return;

        ItemInventory itemInventory = FindItem(item);
        if (itemInventory == null) return;

        ItemInventoryEquipment targetSlot = equipList.Find(slot => slot.slotType == item.itemData.ItemType);
        if (targetSlot == null || targetSlot.HasItem()) return;

        EquipItem(itemInventory, targetSlot);
    }


    private void EquipItem(ItemInventory item, ItemInventoryEquipment slot)
    {
        if (item.stackSize > 1)
        {
            item.RemoveStackSize();
            slot.equipItem = new ItemInventory(item.itemData);
        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());

        }
        else
        {
            slot.equipItem = item;
            ClearFromInventory(item);
        }
        slot.equipItem.AddModifiers(_playerStats);
    }
    
}