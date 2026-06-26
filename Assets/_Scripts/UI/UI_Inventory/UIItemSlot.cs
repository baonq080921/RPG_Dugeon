using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Inventory;

namespace UI
{
    public class UIItemSlot : UiItemSlotBase
    {
        [field: SerializeField] public ItemInventory itemInSlot { get; private set; }

        [SerializeField] private TextMeshProUGUI _tmp;


        protected override void Awake()
        {
            base.Awake();
            _tmp = GetComponentInChildren<TextMeshProUGUI>();
        }

        public virtual void UpdateUISlot(ItemInventory item)
        {
            itemInSlot = item;
            if (item == null)
            {
                image.sprite = deafultSpriteSlot;
                _tmp.text = " ";
            }
            else
            {
                image.sprite = item.itemData.Sprite;
                _tmp.text = item.stackSize >= 1 ? $"{item.stackSize}" : "1";
            }
        }



        private UIItemActionPanel _actionPanel;

        /// <summary>Called by <see cref="UI_Inventory"/> during setup to bind the shared action panel.</summary>
        public void SetActionPanel(UIItemActionPanel panel) => _actionPanel = panel;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (itemInSlot == null || itemInSlot.itemData == null) return;
            _actionPanel?.Show(this);
        }
    }
}
