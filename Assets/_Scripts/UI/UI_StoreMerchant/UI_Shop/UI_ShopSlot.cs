using Base;
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UI_ShopSlot : MonoBehaviour, IPointerDownHandler
    {
        public ItemData itemData;
        [SerializeField] private Image _image;

        void Awake()
        {
            _image = GetComponentInChildren<Image>();
        }
        void Start()
        {
            _image.sprite = itemData.Sprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            EventBus<StoreItemGetInfoEvent>.Raise(new StoreItemGetInfoEvent(itemData));
        }
    }
}
