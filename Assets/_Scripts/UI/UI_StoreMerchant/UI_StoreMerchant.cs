using Base;
using DG.Tweening;
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

        [Header("Player Inventory")]
        [SerializeField] private Transform _inventoryGrid;
        [SerializeField] private PlayerInventory _playerInventory;

        private UIItemSlot[] _inventorySlots;
        private ItemCraftData _currentCraftData;

        private EventBinding<OnInventoryChangedEvent> _inventoryChangedBinding;
        private EventBinding<StoreCallEvent> _storeOpenBinding;
        private EventBinding<CraftGetInfoEvent> _craftInfoBinding;

        private Vector2 _originalPosition;
        private Vector2 _hiddentPosition = new Vector2(9000f, 9000f);

        private void Awake()
        {
            _inventorySlots = _inventoryGrid.GetComponentsInChildren<UIItemSlot>();
            _storeRectTf.anchoredPosition = _hiddentPosition;
            _craftButton.onClick.AddListener(TryCraft);
        }

        private void Start()
        {
            UpdateInventoryDisplay();
        }

        private void OnEnable()
        {
            _inventoryChangedBinding = new EventBinding<OnInventoryChangedEvent>(UpdateInventoryDisplay);
            EventBus<OnInventoryChangedEvent>.Register(_inventoryChangedBinding);

            _storeOpenBinding = new EventBinding<StoreCallEvent>(ShowStoreUI);
            EventBus<StoreCallEvent>.Register(_storeOpenBinding);

            _craftInfoBinding = new EventBinding<CraftGetInfoEvent>(OnCraftInfoReceived);
            EventBus<CraftGetInfoEvent>.Register(_craftInfoBinding);
        }

        private void OnDisable()
        {
            EventBus<OnInventoryChangedEvent>.Deregister(_inventoryChangedBinding);
            EventBus<StoreCallEvent>.Deregister(_storeOpenBinding);
            EventBus<CraftGetInfoEvent>.Deregister(_craftInfoBinding);
            _craftButton.onClick.RemoveListener(TryCraft);
        }

        private void OnCraftInfoReceived(CraftGetInfoEvent e)
        {
            _currentCraftData = e.itemCraftData;
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
            EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
        }

        private void UpdateInventoryDisplay()
        {
            var items = _playerInventory.itemInventoriesList;
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                _inventorySlots[i].UpdateUISlot(i < items.Count ? items[i] : null);
            }
        }

        private void ShowStoreUI()
        {
            Time.timeScale = 0f;
            ServiceLocator.Get<GameManager>().SetPause(true);
            _storeRectTf.DOAnchorPos(_originalPosition, 0.25f).SetUpdate(true).SetEase(Ease.InQuad);
        }

        public void HideStoreUI()
        {
            Time.timeScale = 1f;
            ServiceLocator.Get<GameManager>().SetPause(false);
            _storeRectTf.DOAnchorPos(_hiddentPosition, 0.25f).SetUpdate(true).SetEase(Ease.OutQuad);
        }
    }
}
