using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UiItemSlotBase : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] protected Sprite deafultSpriteSlot;
        [field: SerializeField] protected Image image;



        protected virtual void Awake()
        {
            image = GetComponentInChildren<Image>();
        }

        protected virtual void Start() { }


        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}
