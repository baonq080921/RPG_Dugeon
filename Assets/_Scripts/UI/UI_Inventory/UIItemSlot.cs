
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Base;

public class UIItemSlot : MonoBehaviour, IPointerDownHandler
    {
    [field:SerializeField] public ItemInventory itemInSlot{get; private set;}
    [SerializeField] private Sprite _deafultSpriteSlot;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _tmp;
    public void UpdateUISlot(ItemInventory item)
    {
        itemInSlot = item;
        if(item == null)
        {
            _image.sprite = _deafultSpriteSlot; 
            _tmp.text = " ";
        }
        else
        {
            _image.sprite = item.itemData.Sprite;
            _tmp.text = item.stackSize >=1 ? $"{item.stackSize}": "1";
        }
    }



    private UIItemActionPanel _actionPanel;

    /// <summary>Called by <see cref="UI_Inventory"/> during setup to bind the shared action panel.</summary>
    public void SetActionPanel(UIItemActionPanel panel) => _actionPanel = panel;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null || itemInSlot.itemData == null) return;
        _actionPanel?.Show(this);
    }
}

