using System.Collections.Generic;
using Base;
using DG.Tweening;
using player;
using TMPro;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{

    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private UIItemActionPanel _actionPanel;
    [SerializeField] private UIEquipmentActionPanel _equipmentActionPanel;
    private UIItemSlot[] _uISlots;
    private UIEquipmentSlot[] _uIEquipmentSlots;
    private List<ItemInventory> _items;
    private List<ItemInventoryEquipment> _itemInventoryEquipments;
    private EventBinding<OnInventoryChangedEvent> _eventBindingChanged;
    private EventBinding<AlertNotiEvent> _eventBindingAlert;
    private Sequence _alertSequence;
    [SerializeField] private TextMeshProUGUI _alertNotiTmp;
    


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

    void Start()
    {
        UpdateUIInventory();
    }
    void OnEnable()
    {
        Player.ActivePlayerChanged += OnPlayerChanged;
        _eventBindingChanged = new EventBinding<OnInventoryChangedEvent>(UpdateUIInventory);
        EventBus<OnInventoryChangedEvent>.Register(_eventBindingChanged);
        _eventBindingAlert = new EventBinding<AlertNotiEvent>(AlertNotificationUI);
        EventBus<AlertNotiEvent>.Register(_eventBindingAlert);
    }

    void OnDisable()
    {
        EventBus<OnInventoryChangedEvent>.Deregister(_eventBindingChanged);
        EventBus<AlertNotiEvent>.Deregister(_eventBindingAlert);
        Player.ActivePlayerChanged -= OnPlayerChanged;
    }


    private void OnPlayerChanged(Player player)
    {
        _inventory = player.playerInventory;
        _actionPanel.Setup(_inventory);
        _equipmentActionPanel.Setup(_inventory);
        UpdateUIInventory();
    }

    void AlertNotificationUI(AlertNotiEvent alertNotiEvent)
    {
        _alertNotiTmp.text = alertNotiEvent.alertMessage;
        Vector3 mousePos = Input.mousePosition;
        _alertNotiTmp.rectTransform.position = mousePos;
        _alertNotiTmp.DOKill();
        _alertSequence?.Kill();
        _alertSequence = DOTween.Sequence();
        _alertSequence.Append(_alertNotiTmp.DOFade(1, 0.25f).SetUpdate(true).SetEase(Ease.InBack));
        _alertSequence.AppendInterval(0.5f).SetUpdate(true);
        _alertSequence.Append(_alertNotiTmp.DOFade(0, 0.25f).SetUpdate(true).SetEase(Ease.OutBack));
    }

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
    }
}