using System.Text;
using Base;
using player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Displays item details (name, stats, and crafting materials) when a craft item is selected.
    /// </summary>
    public class UI_ItemDetailsSlot : MonoBehaviour
    {
        private EventBinding<CraftGetInfoEvent> _eventGetCraftInfoBinding;
        private EventBinding<OnInventoryChangedEvent> _inventoryChangedBinding;
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _itemNameTmp;
        [SerializeField] private TextMeshProUGUI _itemStatsTmp;
        [SerializeField] private TextMeshProUGUI _itemSpecialTmp;
        [SerializeField] private TextMeshProUGUI _itemRequiredTmp;
        [SerializeField] private PlayerInventory _playerInventory;

        private ItemCraftData _currentData;

        void OnEnable()
        {
            Player.ActivePlayerChanged += OnPlayerChanged;
            _eventGetCraftInfoBinding = new EventBinding<CraftGetInfoEvent>(UpdateItemDetailsInfo);
            EventBus<CraftGetInfoEvent>.Register(_eventGetCraftInfoBinding);

            _inventoryChangedBinding = new EventBinding<OnInventoryChangedEvent>(RefreshRequirements);
            EventBus<OnInventoryChangedEvent>.Register(_inventoryChangedBinding);
        }

        void OnDisable()
        {
            Player.ActivePlayerChanged -= OnPlayerChanged;
            EventBus<CraftGetInfoEvent>.Deregister(_eventGetCraftInfoBinding);
            EventBus<OnInventoryChangedEvent>.Deregister(_inventoryChangedBinding);
        }

        public void OnPlayerChanged(Player player)
        {
            _playerInventory = player.playerInventory;
        }

        /// <summary>
        /// Updates the item details panel with data from the selected craft item.
        /// </summary>
        public void UpdateItemDetailsInfo(CraftGetInfoEvent craftGetInfoEvent)
        {
            _currentData = craftGetInfoEvent.itemCraftData;
            _itemImage.sprite = _currentData.Sprite;
            _itemNameTmp.text = _currentData.ItemName;
            _itemStatsTmp.text = BuildStatsText(_currentData.modifiers);
            _itemRequiredTmp.text = BuildRequirementsText(_currentData.requirementItems);
            _itemSpecialTmp.text = craftGetInfoEvent.itemCraftData.itemEffectData?.effectInfo ?? string.Empty;
        }

        private void RefreshRequirements()
        {
            if (_currentData == null) return;
            _itemRequiredTmp.text = BuildRequirementsText(_currentData.requirementItems);
        }

        private string BuildStatsText(ItemModifier[] modifiers)
        {
            if (modifiers == null || modifiers.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (ItemModifier modifier in modifiers)
            {
                sb.AppendLine($"+{modifier.value:0.##} {modifier.statType}");
            }
            return sb.ToString().TrimEnd();
        }

        private string BuildRequirementsText(RequirementItem[] requirementItems)
        {
            if (requirementItems == null || requirementItems.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (RequirementItem item in requirementItems)
            {
                int required = (int)item.amount;
                int playerAmount = GetInventoryCountByType(item.itemTypes);
                string color = playerAmount >= required ? "green" : "red";
                sb.AppendLine($"<color={color}>{item.itemTypes} {playerAmount}/{required}</color>");
            }
            return sb.ToString().TrimEnd();
        }

        private int GetInventoryCountByType(ItemTypes itemType)
        {
            int total = 0;
            foreach (ItemInventory item in _playerInventory.itemInventoriesList)
            {
                if (item.itemData.ItemType == itemType)
                    total += item.stackSize;
            }
            return total;
        }
    }
}
