using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contextual action panel for equipment slots.
/// Shows Unequip (return to inventory) and Drop (discard) options.
/// </summary>
public class UIEquipmentActionPanel : MonoBehaviour
{
    [SerializeField] private Button _unequipButton;
    [SerializeField] private Button _dropButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _itemNameTmp;
    [SerializeField] private TextMeshProUGUI _itemText;

    private UIEquipmentSlot _currentSlot;
    private PlayerInventory _inventory;
    private Canvas _canvas;
    private RectTransform _panelRT;

    private void Awake()
    {
        _panelRT = (RectTransform)transform;
        _canvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    /// <summary>Called once by <see cref="UI_Inventory"/> to bind the inventory this panel acts on.</summary>
    public void Setup(PlayerInventory inventory)
    {
        _inventory = inventory;
        gameObject.SetActive(false);
    }

    /// <summary>Shows the action panel near the given equipment slot.</summary>
    public void Show(UIEquipmentSlot slot)
    {
        if (slot.ItemEquip == null || !slot.ItemEquip.HasItem()) return;

        _currentSlot = slot;
        gameObject.SetActive(true);

        _unequipButton.onClick.RemoveAllListeners();
        _dropButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();

        _unequipButton.onClick.AddListener(OnUnequip);
        _dropButton.onClick.AddListener(OnDrop);
        _closeButton.onClick.AddListener(Hide);

        if (_itemNameTmp != null)
            _itemNameTmp.text = slot.ItemEquip.equipItem.itemData.ItemName;
        UpdateModifierText(slot.ItemEquip);
        PositionNearSlot(slot);
    }

    /// <summary>Hides the panel without taking any action.</summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        _currentSlot = null;
    }

    private void PositionNearSlot(UIEquipmentSlot slot)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform slotRT = (RectTransform)slot.transform;
        float scale = _canvas.scaleFactor;

        Camera canvasCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        Vector2 slotScreen = RectTransformUtility.WorldToScreenPoint(canvasCamera, slotRT.position);

        Vector2 panelSize = _panelRT.rect.size * scale;
        Vector2 slotSize = slotRT.rect.size * scale;

        float x = slotScreen.x + slotSize.x * 0.5f + panelSize.x * 0.5f;
        float y = slotScreen.y;

        if (x + panelSize.x * 0.5f > Screen.width)
            x = slotScreen.x - slotSize.x * 0.5f - panelSize.x * 0.5f;

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

    private void UpdateModifierText(ItemInventoryEquipment item)
    {
        if (_itemText == null) return;

        if(item?.equipItem.itemData is EquipmentData equipment)
        {
            var sb = new StringBuilder();
            foreach (var mod in equipment.modifiers)
            {
                string sign = mod.value >= 0 ? "-" : string.Empty;
                sb.AppendLine($"{mod.statType}: {sign}{mod.value}");
            }
            _itemText.text = sb.ToString().TrimEnd();
        }

        
    }

    private void OnUnequip()
    {
        if (_currentSlot?.ItemEquip == null) return;
        _inventory.UnequipItem(_currentSlot.ItemEquip);
        Hide();
    }

    private void OnDrop()
    {
        if (_currentSlot?.ItemEquip == null) return;
        _inventory.DropEquippedItem(_currentSlot.ItemEquip);
        Hide();
    }
}
