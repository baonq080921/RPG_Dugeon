using Base;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UI
{    
    public class UI_CatergoriestSlot : MonoBehaviour,IPointerDownHandler
    {
        [SerializeField] private ItemCraftData _itemCraft;
        [SerializeField] private TextMeshProUGUI _itemNameTmp;
        [SerializeField] private Image _itemCraftIcon;

        public void OnPointerDown(PointerEventData eventData)
        {
            EventBus<CraftGetInfoEvent>.Raise(new CraftGetInfoEvent(_itemCraft));
        }

        void OnValidate()
        {
            if(_itemCraft == null) return;
            _itemNameTmp.text = _itemCraft.ItemName;
            _itemCraftIcon.sprite = _itemCraft.Sprite;
            gameObject.name = _itemCraft.name;
        }
    }
}