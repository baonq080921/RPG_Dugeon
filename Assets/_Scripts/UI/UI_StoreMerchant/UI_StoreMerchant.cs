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
        [Header("Player Inventory")]
        [SerializeField] private Transform _inventoryGrid;
        [SerializeField] private PlayerInventory _playerInventory;

        private UIItemSlot[] _inventorySlots;
        private EventBinding<OnInventoryChangedEvent> _inventoryChangedBinding;
        private EventBinding<StoreCallEvent> _storeOpenBinding;

        private Vector2 _originalPosition;
        private Vector2 _hiddentPosition = new Vector2(9000f,9000f);
        private void Awake()
        {
            _inventorySlots = _inventoryGrid.GetComponentsInChildren<UIItemSlot>();
            _storeRectTf.anchoredPosition = _hiddentPosition; 
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
        }

        private void OnDisable()
        {
            EventBus<OnInventoryChangedEvent>.Deregister(_inventoryChangedBinding);
            EventBus<StoreCallEvent>.Deregister(_storeOpenBinding);
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
            _storeRectTf.DOAnchorPos(_originalPosition,0.25f).SetUpdate(true).SetEase(Ease.InQuad);
        }

        public void HideStoreUI()
        {
            Time.timeScale = 1f;
            ServiceLocator.Get<GameManager>().SetPause(false);
            _storeRectTf.DOAnchorPos(_hiddentPosition,0.25f).SetUpdate(true).SetEase(Ease.OutQuad);
        }
    }
}
