using System.Collections.Generic;
using Base;
using DG.Tweening;
using player;
using TMPro;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{

    private PlayerInventory _inventory;
    [SerializeField] private UIItemActionPanel _actionPanel;
    [SerializeField] private UIEquipmentActionPanel _equipmentActionPanel;
    private UIItemSlot[] _uISlots;
    private UIEquipmentSlot[] _uIEquipmentSlots;
    private List<ItemInventory> _items;
    private List<ItemInventoryEquipment> _itemInventoryEquipments;
    private EventBinding<OnInventoryChangedEvent> _eventBindingChanged;
    [SerializeField] private TextMeshProUGUI _playerGoldText;
    


    void Awake()
    {
        _uISlots = GetComponentsInChildren<UIItemSlot>();
        _uIEquipmentSlots = GetComponentsInChildren<UIEquipmentSlot>();
        if (_inventory != null)
        {
            _actionPanel.Setup(_inventory);
            _equipmentActionPanel.Setup(_inventory);
        }
        foreach (var slot in _uISlots)
            slot.SetActionPanel(_actionPanel);
        foreach (var slot in _uIEquipmentSlots)
            slot.SetActionPanel(_equipmentActionPanel);
    }

    void OnEnable()
    {
        Player.ActivePlayerChanged += OnPlayerChanged;
        if (Player.ActivePlayer != null)
            OnPlayerChanged(Player.ActivePlayer);
        _eventBindingChanged = new EventBinding<OnInventoryChangedEvent>(UpdateUIInventory);
        EventBus<OnInventoryChangedEvent>.Register(_eventBindingChanged);
    }

    void OnDisable()
    {
        EventBus<OnInventoryChangedEvent>.Deregister(_eventBindingChanged);
        Player.ActivePlayerChanged -= OnPlayerChanged;
    }


    private void OnPlayerChanged(Player player)
    {
        _inventory = player.playerInventory;
        _actionPanel.Setup(_inventory);
        _equipmentActionPanel.Setup(_inventory);
        UpdateUIInventory();
    }

    private void UpateCurrentPlayerGoldUI(float amount) =>_playerGoldText.text = BuildText(amount);


    void UpdateUIInventory()
    {
        if (_inventory == null) return;
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

        UpateCurrentPlayerGoldUI(_inventory.Money);
    }

    private string BuildText(float amount) => $"Your gold: <color=green>{amount}</color>";
}