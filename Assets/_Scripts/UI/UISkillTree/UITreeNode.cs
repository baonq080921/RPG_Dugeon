using UnityEngine;
using Base;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private SkillTreeData _skillTreeData;

    [SerializeField] private Image _iconImage;
    [Header("Tooltip Animation")]
    [SerializeField] private float _tooltipDuration = 0.25f;
    [SerializeField] private float _tooltipSlideOffset = 30f;

    private string _hexadecimalLockedColor = "#544F4F";
    private string _hexadecimalHoverColor = "#FAFC06";
    private Color _lastColor;
    public bool isUnlocked { get; private set; }
    public bool isLocked { get; private set; }

    void OnValidate()
    {
        if (_skillTreeData == null)
            return;
        gameObject.name = "UI Tree Node - " + _skillTreeData.skillTreeDataName;
        if (_iconImage != null)
            _iconImage.sprite = _skillTreeData.Icon;
    }

    void Awake()
    {
        UpdateIconColor(GetColorByHex(_hexadecimalLockedColor));

    }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
    }

    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)
            return false;
        return true;
    }

    private void UpdateIconColor(Color color)
    {
        if (_iconImage == null)
            return;
        _iconImage.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
            Unlock();
        else
            DebugCustom.Log("Can't unlock this node yet.");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DebugCustom.Log("Show Text about this Skill");
        ServiceLocator.Get<UIManager>().uISkillToolTip.ShowToolTip(true, _rect, _skillTreeData);
        if (isUnlocked)
            return;
        _lastColor = _iconImage.color;
        UpdateIconColor(GetColorByHex(_hexadecimalHoverColor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DebugCustom.Log("Hide Text about this Skill");
        ServiceLocator.Get<UIManager>().uISkillToolTip.ShowToolTip(false, _rect,_skillTreeData);
        if (isUnlocked)
            return;
        UpdateIconColor(_lastColor);
    }

    private Color GetColorByHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var color);
        color.a = 1f;
        return color;
    }
}
