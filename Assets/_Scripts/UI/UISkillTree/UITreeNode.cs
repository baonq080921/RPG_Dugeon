using System.Collections;
using UnityEngine;
using Base;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using player;

public class UITreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform _rect;
    public SkillTreeData skillTreeData;
    [Header("Require and Confilct Node")]
    public UITreeNode[] requireNode;
    public UITreeNode[] confilctNode;

    [Header("UI Node details")]
    [SerializeField] private Image _iconImage;
    private Color _lockedColor;
    private Color _hoverColor;
    private Color _lastColor;

    [Header("Cannot Unlock Feedback")]
    [SerializeField] private Color _cannotUnlockColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private float _shakeDuration = 0.4f;
    [SerializeField] private float _shakeStrength = 10f;
    [SerializeField] private int _shakeCount = 6;
    private Coroutine _feedbackCoroutine;
    private Vector2 _preFeedbackPos;
    private bool _isHovering;
    [field:SerializeField]public bool isUnlocked { get; private set; }
    [field:SerializeField]public bool isLocked { get; private set; }

    void OnValidate()
    {
        if (skillTreeData == null)
            return;
        gameObject.name = "UI Tree Node - " + skillTreeData.skillTreeDataName;
        if (_iconImage != null)
            _iconImage.sprite = skillTreeData.Icon;
    }

    void Awake()
    {
        _lockedColor = GetColorByHex("#544F4F");
        _hoverColor  = GetColorByHex("#FAFC06");
        UpdateIconColor(_lockedColor);
    }

    // void Start()
    // {
    //     if (skillTreeData != null && skillTreeData.Cost == 0)
    //         Unlock();
    // }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        ServiceLocator.Get<UIManager>().uISkillTree.ReduceSkillPoint(skillTreeData.Cost);
        LockOtherSkillPath();
        RefreshParentLineColors();

        ServiceLocator.Get<PlayerSkillManager>().GetSkillByType(skillTreeData.SkillType).SetSkillUpgradeType(skillTreeData.UpgradeData.SkillUpgrade);
        ServiceLocator.Get<PlayerSkillManager>().GetSkillByType(skillTreeData.SkillType).SetUpgradeForSkill(skillTreeData.UpgradeData);
    }

    private void RefreshParentLineColors()
    {
        foreach(var parent in requireNode)
            parent.GetComponent<UIConnectedHandler>()?.RefreshLineColors();
    }

    private bool CanBeUnlocked()
    {
        bool isEnoughSkillPoint = ServiceLocator.Get<UIManager>().uISkillTree.EnoughSkillPoint(skillTreeData.Cost);
        if (isLocked || isUnlocked)
            return false;
        if(!isEnoughSkillPoint) return false;


        foreach(var node in requireNode)
            if(!node.isUnlocked) return false;

        foreach(var node in confilctNode)
            if(node.isUnlocked) return false;

        return true;
    }


    /// <summary>
    /// Lock the other skill path if player decided to choose one path to develope skill
    /// </summary> <summary>
    /// 
    /// </summary>
    private void LockOtherSkillPath()
    {
        foreach(var node in confilctNode)
            node.NotifyLocked();
    }

    /// <summary>Called by a conflicting node when it unlocks, permanently locking this node.</summary>
    public void NotifyLocked()
    {
        isLocked = true;
        if (_feedbackCoroutine != null)
        {
            StopCoroutine(_feedbackCoroutine);
            _rect.anchoredPosition = _preFeedbackPos;
            _feedbackCoroutine = null;
        }
        UpdateIconColor(_lockedColor);
    }

    /// <summary>Resets this node to its initial locked state without refunding points (refund is handled by UISkillTree).</summary>
    public void ResetNode()
    {
        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(_lockedColor);
    }

    private void UpdateIconColor(Color color)
    {
        if (_iconImage == null)
            return;
        _iconImage.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked()) { Unlock(); return; }
        if (isUnlocked) return;

        if (_feedbackCoroutine != null)
        {
            StopCoroutine(_feedbackCoroutine);
            _rect.anchoredPosition = _preFeedbackPos; // reset position before restarting
        }
        _preFeedbackPos = _rect.anchoredPosition;
        _feedbackCoroutine = StartCoroutine(CannotUnlockFeedback());
    }

    private IEnumerator CannotUnlockFeedback()
    {
        float stepDuration = _shakeDuration / _shakeCount;

        for (int i = 0; i < _shakeCount; i++)
        {
            float dir = (i % 2 == 0) ? 1f : -1f;
            _rect.anchoredPosition = _preFeedbackPos + new Vector2(_shakeStrength * dir, 0f);
            _iconImage.color = _cannotUnlockColor;
            yield return new WaitForSeconds(stepDuration * 0.5f);
            _rect.anchoredPosition = _preFeedbackPos;
            _iconImage.color = _isHovering ? _hoverColor : _lockedColor;
            yield return new WaitForSeconds(stepDuration * 0.5f);
        }

        _rect.anchoredPosition = _preFeedbackPos;
        _feedbackCoroutine = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        ServiceLocator.Get<UIManager>()?.uISkillToolTip?.ShowToolTip(true, _rect, this);
        if (isUnlocked) return;
        _lastColor = _iconImage.color;
        UpdateIconColor(_hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        ServiceLocator.Get<UIManager>()?.uISkillToolTip?.ShowToolTip(false, _rect, this);
        if (isUnlocked) return;

        if (_feedbackCoroutine != null)
        {
            StopCoroutine(_feedbackCoroutine);
            _rect.anchoredPosition = _preFeedbackPos;
            _feedbackCoroutine = null;
        }
        UpdateIconColor(_lastColor);
    }

    private Color GetColorByHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var color);
        color.a = 1f;
        return color;
    }
}
