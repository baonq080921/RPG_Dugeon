using System.Collections.Generic;
using Base;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inventory : MonoBehaviour
{

    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private UIItemActionPanel _actionPanel;
    private UIItemSlot[] _uISlots;
    private UIEquipmentSlot[] _uIEquipmentSlots;
    private List<ItemInventory> _items;
    private List<ItemInventoryEquipment> _itemInventoryEquipments;
    private EventBinding<OnInventoryChangedEvent> _eventBindingChanged;
    

    void Awake()
    {
        _uISlots = GetComponentsInChildren<UIItemSlot>();
        _uIEquipmentSlots = GetComponentsInChildren<UIEquipmentSlot>();
        _actionPanel.Setup(_inventory);
        foreach (var slot in _uISlots)
            slot.SetActionPanel(_actionPanel);
    }

    void Start()
    {
        UpdateUIInventory();
    }
    void OnEnable()
    {
        _eventBindingChanged = new EventBinding<OnInventoryChangedEvent>(UpdateUIInventory);
        EventBus<OnInventoryChangedEvent>.Register(_eventBindingChanged);
    }

    void OnDisable()
    {
        EventBus<OnInventoryChangedEvent>.Deregister(_eventBindingChanged);
    }


    void UpdateUIInventory()
    {
        _items = _inventory.itemInventoriesList;
        _itemInventoryEquipments = _inventory.equipList;

        for (int i = 0; i < _uISlots.Length; i++)
        {
            if (i < _items.Count) _uISlots[i].UpdateUISlot(_items[i]);
            else _uISlots[i].UpdateUISlot(null);
        }

        foreach (var slot in _uIEquipmentSlots)
        {
            ItemInventoryEquipment match = _itemInventoryEquipments.Find(e => e.slotType == slot.SlotType);
            slot.UpdateUISlot(match);
        }
    }
}