using System;
using System.Text;
using Base;
using enemy;
using player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Displays the full details of a shop item — icon, name, type, stats, special effect, and gold cost —
    /// and handles the buy interaction.
    /// </summary>
    public class UI_ShopItemDetails : MonoBehaviour
    {
        private EventBinding<StoreItemGetInfoEvent> _storeItemInfoBinding;
        private EventBinding<OnInventoryChangedEvent> _inventoryChangedBinding;

        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _itemNameTmp;
        [SerializeField] private TextMeshProUGUI _itemTypeTmp;
        [SerializeField] private TextMeshProUGUI _itemStatsTmp;
        [SerializeField] private TextMeshProUGUI _itemSpecialTmp;
        [SerializeField] private TextMeshProUGUI _itemCostTmp;
        [SerializeField] private TextMeshProUGUI _playerMoneyTmp;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _rectTf;
        [SerializeField] private CanvasGroup _shopSlotsCanvasGroup;
        private PlayerInventory _playerInventory;
        private ItemData _currentItem;
        private Vector2 _hiddentPosition = new Vector2(9000f, 9000f);


        void OnEnable()
        {
            Player.ActivePlayerChanged += OnPlayerChanged;
            if (Player.ActivePlayer != null)
                OnPlayerChanged(Player.ActivePlayer);
            _storeItemInfoBinding = new EventBinding<StoreItemGetInfoEvent>(OnStoreItemInfo);
            EventBus<StoreItemGetInfoEvent>.Register(_storeItemInfoBinding);
            _inventoryChangedBinding = new EventBinding<OnInventoryChangedEvent>(RefreshAffordability);
            EventBus<OnInventoryChangedEvent>.Register(_inventoryChangedBinding);
            _buyButton.onClick.AddListener(TryBuyItem);
            _closeButton.onClick.AddListener(HideTheStoreItemInfo);
        }

        void OnDisable()
        {
            Player.ActivePlayerChanged -= OnPlayerChanged;
            EventBus<StoreItemGetInfoEvent>.Deregister(_storeItemInfoBinding);
            EventBus<OnInventoryChangedEvent>.Deregister(_inventoryChangedBinding);
            _buyButton.onClick.RemoveListener(TryBuyItem);
            _closeButton.onClick.RemoveListener(HideTheStoreItemInfo);
        }

        /// <summary>Wires up the player inventory reference when the active player changes.</summary>
        public void OnPlayerChanged(Player player)
        {
            _playerInventory = player.playerInventory;
            RefreshAffordability();
        }

        private void OnStoreItemInfo(StoreItemGetInfoEvent e)
        {
            var camera = ServiceLocator.Get<Helper>().mainCam;
            _currentItem = e.itemData;
            _rectTf.anchoredPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            _shopSlotsCanvasGroup.blocksRaycasts = false;
            UpdateDisplay();
        }

        private void HideTheStoreItemInfo()
        {
            _rectTf.anchoredPosition = _hiddentPosition;
            _shopSlotsCanvasGroup.blocksRaycasts = true;
        }
        private void UpdateDisplay()
        {
            if (_currentItem == null) return;

            _itemImage.sprite = _currentItem.Sprite;
            _itemNameTmp.text = _currentItem.ItemName;
            _itemTypeTmp.text = BuildTypeText(_currentItem);
            _itemStatsTmp.text = BuildStatsText(_currentItem);
            _itemSpecialTmp.text = _currentItem.itemEffectData?.effectInfo ?? string.Empty;
            _itemCostTmp.text = $"{_currentItem.money:0} Gold";
            RefreshAffordability();
        }

        private void RefreshAffordability()
        {
            if (_playerInventory != null)
                _playerMoneyTmp.text = $"Your Gold: {_playerInventory.Money:0}";

            bool canAfford = _playerInventory != null && _currentItem != null
                             && _playerInventory.CanSpendMoney(_currentItem.money);
            _buyButton.interactable = canAfford;
        }

        /// <summary>
        /// Attempts to buy the currently displayed item. Deducts gold and adds the item to inventory.
        /// Raises an <see cref="AlertNotiEvent"/> if the player cannot afford it or the inventory is full.
        /// </summary>
        public void TryBuyItem()
        {
            if (_currentItem == null || _playerInventory == null) return;
            var mousePos = ServiceLocator.Get<Helper>().mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (!_playerInventory.CanSpendMoney(_currentItem.money))
            {
                EventBus<AlertNotiEvent>.Raise(new AlertNotiEvent(GameMessages.Alert("Not Enough Gold"), mousePos, Color.red));
                return;
            }

            ItemInventory newItem = new ItemInventory(_currentItem);
            bool hasStackableSlot = _playerInventory.FindItem(newItem)?.CanAddToStack() == true;
            if (!_playerInventory.CanAddToIventory() && !hasStackableSlot)
            {
                EventBus<AlertNotiEvent>.Raise(new AlertNotiEvent(GameMessages.Alert("Inventory Full"), mousePos, Color.red));
                return;
            }

            _playerInventory.SpendMoney(_currentItem.money);
            _playerInventory.AddToInventory(newItem);
            HideTheStoreItemInfo();
            RefreshAffordability();
            EventBus<AlertNotiEvent>.Raise(new AlertNotiEvent(GameMessages.Alert("Buy Success"), mousePos, Color.green));
        }

        private string BuildTypeText(ItemData item)
        {
            if (item.EquipSlot != EquipSlotType.None)
                return $"{item.ItemType} | {item.EquipSlot}";
            return item.ItemType.ToString();
        }

        private string BuildStatsText(ItemData item)
        {
            if (item is not EquipmentData equipment
                || equipment.modifiers == null
                || equipment.modifiers.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (ItemModifier modifier in equipment.modifiers)
                sb.AppendLine($"+{modifier.value:0.##} {modifier.statType}");
            return sb.ToString().TrimEnd();
        }
    }
}
