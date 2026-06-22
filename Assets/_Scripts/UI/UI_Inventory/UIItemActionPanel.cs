using System.Text;
using Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contextual action panel that appears near a tapped inventory slot, like a tooltip.
/// Positions itself to the right of the slot by default and flips/clamps when near screen edges.
/// </summary>
public class UIItemActionPanel : MonoBehaviour
{
    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _dropButton;
    [SerializeField] private Button _closeButton;

    private UIItemSlot _currentSlot;
    private InventoryBase _inventory;
    private Canvas _canvas;
    private RectTransform _panelRT;
    [SerializeField] private TextMeshProUGUI _skillTmp;
    [SerializeField] private TextMeshProUGUI _itemNameTmp;

    private void Awake()
    {
        _panelRT = (RectTransform)transform;
        _canvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    /// <summary>Called once by <see cref="UI_Inventory"/> to bind the inventory this panel acts on.</summary>
    public void Setup(InventoryBase inventory)
    {
        _inventory = inventory;
        gameObject.SetActive(false);
    }

    /// <summary>Shows the action panel near the given slot, repositioning away from screen edges.</summary>
    public void Show(UIItemSlot slot)
    {
        _currentSlot = slot;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        bool isEquippable;
        if(slot.itemInSlot?.itemData is EquipmentData || slot.itemInSlot?.itemData is ItemUse)
            isEquippable = true;
        else
            isEquippable = false;

        _equipButton.gameObject.SetActive(isEquippable);

        _equipButton.onClick.RemoveAllListeners();
        _dropButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();
        if (isEquippable)
            _equipButton.onClick.AddListener(OnEquip);
        _dropButton.onClick.AddListener(OnDrop);
        _closeButton.onClick.AddListener(Hide);

        if (_itemNameTmp != null)
            _itemNameTmp.text = slot.itemInSlot?.itemData?.ItemName ?? string.Empty;

        UpdateModifierText(slot.itemInSlot);
        PositionNearSlot(slot);
    }

    /// <summary>Hides the panel without taking any action.</summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        _currentSlot = null;
    }

    private void UpdateModifierText(ItemInventory item)
    {
        if (_skillTmp == null) return;

        if (item?.itemData is not EquipmentData equipment || equipment.modifiers == null || equipment.modifiers.Length == 0)
        {
            _skillTmp.text = string.Empty;
            return;
        }

        var sb = new StringBuilder();
        foreach (var mod in equipment.modifiers)
        {
            string sign = mod.value >= 0 ? "+" : string.Empty;
            sb.AppendLine($"{mod.statType}: {sign}{mod.value}");
        }
        sb.AppendLine($"{item.itemData.describleItem}");
        
        _skillTmp.text = sb.ToString().TrimEnd();
    }

    private void PositionNearSlot(UIItemSlot slot)
    {
        // Force layout rebuild so _panelRT.rect.size is accurate after SetActive
        Canvas.ForceUpdateCanvases();

        RectTransform slotRT = (RectTransform)slot.transform;
        float scale = _canvas.scaleFactor;

        Camera canvasCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        Vector2 slotScreen = RectTransformUtility.WorldToScreenPoint(canvasCamera, slotRT.position);

        Vector2 panelSize = _panelRT.rect.size * scale;
        Vector2 slotSize  = slotRT.rect.size * scale;

        // Default: place to the right of the slot
        float x = slotScreen.x + slotSize.x * 0.5f + panelSize.x * 0.5f;
        float y = slotScreen.y;

        // Flip to the left if the right edge would overflow
        if (x + panelSize.x * 0.5f > Screen.width)
            x = slotScreen.x - slotSize.x * 0.5f - panelSize.x * 0.5f;

        // Clamp so panel never clips top or bottom
        y = Mathf.Clamp(y, panelSize.y * 0.5f, Screen.height - panelSize.y * 0.5f);

        SetScreenPosition(new Vector2(x, y));
    }

    private void SetScreenPosition(Vector2 screenPos)
    {
        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _panelRT.position = new Vector3(screenPos.x, screenPos.y, 0f);
            return;
        }

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)_canvas.transform, screenPos, _canvas.worldCamera, out Vector3 worldPos);
        _panelRT.position = worldPos;
    }

    private void OnEquip()
    {
        if (_currentSlot?.itemInSlot == null) return;
        EventBus<EquipEvent>.Raise(new EquipEvent(_currentSlot.itemInSlot));
        Hide();
    }

    private void OnDrop()
    {
        if (_currentSlot?.itemInSlot == null || _inventory == null) return;
        _inventory.DropItem(_currentSlot.itemInSlot);
        Hide();
    }
}