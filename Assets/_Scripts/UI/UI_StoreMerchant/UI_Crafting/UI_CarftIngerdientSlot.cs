using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_CarftIngerdientSlot : MonoBehaviour
    {
        [SerializeField] private Image _slotImage;
        [SerializeField] private Sprite _spriteDefault;
        [SerializeField] private TextMeshProUGUI _amountText;
        private ItemTypes _itemTypes;
        public void UpdateUiSlot(RequirementItem requirementItem, float amount = 0)
        {
            _slotImage.sprite = requirementItem != null ? requirementItem.sprite : _spriteDefault;
            _amountText.text = amount != 0 ? amount.ToString() : "";
        }

    }
}
