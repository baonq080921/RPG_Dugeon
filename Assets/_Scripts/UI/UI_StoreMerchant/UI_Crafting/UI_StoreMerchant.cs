using Base;
using DG.Tweening;
using InteractiveObject;
using Inventory;
using player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Controls the merchant store UI: category list on the left, item info and craft panel on the top-right,
    /// and the player's inventory display on the bottom-right.
    /// </summary>
    public class UI_StoreMerchant : MonoBehaviour
    {
        [Header("Categories")]
        [SerializeField] private RectTransform _storeRectTf;

        [Header("Item Info")]
        [SerializeField] private Image _selectedItemIcon;
        [SerializeField] private TextMeshProUGUI _selectedItemName;
        [SerializeField] private TextMeshProUGUI _selectedItemStats;
        [SerializeField] private Transform _materialsContainer;
        [SerializeField] private Button _craftButton;

        [Header("Crafting")]
        [SerializeField] private ItemObjectPickable _itemPickablePrefab;
        [SerializeField] private UI_CarftIngerdientSlot[] _carftIngerdientSlots;

        [Header("Player Inventory")]
        [SerializeField] private Transform _inventoryGrid;
        [SerializeField] private PlayerInventory _playerInventory;

        private UIItemSlot[] _inventorySlots;
        private ItemCraftData _currentCraftData;
        private EventBinding<OnInventoryChangedEvent> _inventoryChangedBinding;
        private EventBinding<CraftStoreCallEvent> _storeOpenBinding;
        private EventBinding<CraftGetInfoEvent> _craftInfoBinding;

        private Vector2 _originalPosition;
        private Vector2 _hiddentPosition = new Vector2(9000f, 9000f);

        private void Awake()
        {
            _inventorySlots = _inventoryGrid.GetComponentsInChildren<UIItemSlot>();
            _carftIngerdientSlots = GetComponentsInChildren<UI_CarftIngerdientSlot>();
            _storeRectTf.anchoredPosition = _hiddentPosition;
            _craftButton.onClick.AddListener(TryCraft);
        }

        private void OnEnable()
        {
            Player.ActivePlayerChanged += OnPlayerChanged;
            if (Player.ActivePlayer != null)
                OnPlayerChanged(Player.ActivePlayer);
            _inventoryChangedBinding = new EventBinding<OnInventoryChangedEvent>(UpdateInventoryDisplay);
            EventBus<OnInventoryChangedEvent>.Register(_inventoryChangedBinding);

            _storeOpenBinding = new EventBinding<CraftStoreCallEvent>(ShowStoreUI);
            EventBus<CraftStoreCallEvent>.Register(_storeOpenBinding);

            _craftInfoBinding = new EventBinding<CraftGetInfoEvent>(OnCraftInfoReceived);
            EventBus<CraftGetInfoEvent>.Register(_craftInfoBinding);
        }

        private void OnDisable()
        {
            Player.ActivePlayerChanged -= OnPlayerChanged;
            EventBus<OnInventoryChangedEvent>.Deregister(_inventoryChangedBinding);
            EventBus<CraftStoreCallEvent>.Deregister(_storeOpenBinding);
            EventBus<CraftGetInfoEvent>.Deregister(_craftInfoBinding);
            _craftButton.onClick.RemoveListener(TryCraft);
        }

        private void OnCraftInfoReceived(CraftGetInfoEvent e)
        {
            _currentCraftData = e.itemCraftData;
            UpdateCraftIngerdientSlotDisplay();
        }

        private void TryCraft()
        {
            if (_currentCraftData == null) return;

            foreach (RequirementItem req in _currentCraftData.requirementItems)
            {
                if (_playerInventory.GetCountByType(req.itemTypes) < (int)req.amount)
                    return;
            }

            foreach (RequirementItem req in _currentCraftData.requirementItems)
                _playerInventory.ConsumeByType(req.itemTypes, (int)req.amount);

            Vector3 spawnPos = _playerInventory.transform.position +new Vector3(5f,0f,0f); 
            ItemObjectPickable spawned = Instantiate(_itemPickablePrefab, spawnPos, Quaternion.identity);
            spawned.Initialize(_currentCraftData);
            spawned.ShootItem();
            EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());

        }

        private void OnPlayerChanged(Player player)
        {
            _playerInventory = player.playerInventory;
            UpdateInventoryDisplay();
        }

        private void UpdateInventoryDisplay()
        {
            if (_playerInventory == null) return;
            var items = _playerInventory.itemInventoriesList;
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                _inventorySlots[i].UpdateUISlot(i < items.Count ? items[i] : null);
            }
        }

        private void UpdateCraftIngerdientSlotDisplay()
        {
            for(int i = 0 ; i< _carftIngerdientSlots.Length; i++)
            {
                if(i < _currentCraftData.requirementItems.Length)
                {
                    var item = _currentCraftData.requirementItems[i];
                    
                    _carftIngerdientSlots[i].UpdateUiSlot(item,item.amount);
                }
                else 
                    _carftIngerdientSlots[i].UpdateUiSlot(null);

            }
        }

        private void ShowStoreUI()
        {
            ServiceLocator.Get<GameManager>().SetPause(true);
            _storeRectTf.DOAnchorPos(_originalPosition, 0.25f).SetUpdate(true).SetEase(Ease.InQuad);
            EventBus<OnToggleButtonUIEvent>.Raise(new OnToggleButtonUIEvent(true));
        }

        public void HideStoreUI()
        {
            ServiceLocator.Get<GameManager>().SetPause(false);
            _storeRectTf.DOAnchorPos(_hiddentPosition, 0.25f).SetUpdate(true).SetEase(Ease.OutQuad);
            EventBus<OnToggleButtonUIEvent>.Raise(new OnToggleButtonUIEvent(false));
        }
    }
}
