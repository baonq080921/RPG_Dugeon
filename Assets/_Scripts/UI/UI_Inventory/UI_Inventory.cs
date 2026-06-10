using System.Collections.Generic;
using Base;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inventory : MonoBehaviour
{

    [SerializeField] private InventoryBase _inventory;
    [SerializeField] private UIItemActionPanel _actionPanel;
    private UIItemSlot[] uISlots;

    private List<ItemInventory> items;
    private EventBinding<OnInventoryChangedEvent> eventBindingChanged;
    

    void Awake()
    {
        uISlots = GetComponentsInChildren<UIItemSlot>();
        _actionPanel.Setup(_inventory);
        foreach (var slot in uISlots)
            slot.SetActionPanel(_actionPanel);
    }

    void Start()
    {
        UpdateUIInventory();
    }
    void OnEnable()
    {
        eventBindingChanged = new EventBinding<OnInventoryChangedEvent>(UpdateUIInventory);
        EventBus<OnInventoryChangedEvent>.Register(eventBindingChanged);
    }

    void OnDisable()
    {
        EventBus<OnInventoryChangedEvent>.Deregister(eventBindingChanged);
    }


    void UpdateUIInventory()
    {
        items = _inventory.itemInventoriesList;
        for(int i = 0 ; i< uISlots.Length; i++)
        {
            if(i < items.Count) uISlots[i].UpdateUISlot(items[i]);
            else uISlots[i].UpdateUISlot(null);
        }
    }
}