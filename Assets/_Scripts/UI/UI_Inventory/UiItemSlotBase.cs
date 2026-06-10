using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiItemSlotBase : MonoBehaviour, IPointerDownHandler
{
     [SerializeField] protected Sprite deafultSpriteSlot;
    [field:SerializeField] protected Image image;

    

    protected virtual void Awake()
    {
        image = GetComponentInChildren<Image>();
    }


    public virtual void OnPointerDown(PointerEventData eventData)
    {
    }
}