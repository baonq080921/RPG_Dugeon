using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
