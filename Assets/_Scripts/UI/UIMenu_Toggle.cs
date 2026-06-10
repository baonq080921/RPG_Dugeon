using Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenu_Toggle : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RectTransform _uiRectTf;
    private Vector2 _hiddenPosiiton = new Vector2(9999f,9999f);
    private Vector2 _originalPosition;
    [SerializeField] private RectTransform _buttonsRectTf;
    private bool isOpen = false;
    private Tween _moveTween;

    public void OnPointerDown(PointerEventData eventData)
    {
        isOpen = !isOpen;
        ToggleUIMenu(isOpen);

    }

    void Awake()
    {
        _originalPosition = _uiRectTf.anchoredPosition;
    }

    void Start()
    {
        ToggleUIMenu(false);
    }
    private void ToggleUIMenu(bool open)
    {
        _uiRectTf.anchoredPosition = open ? _originalPosition : _hiddenPosiiton;
        Time.timeScale = open ? 0 : 1;
        ServiceLocator.Get<GameManager>()?.SetPause(open);
        ToggleButtonsCanvas(open);
    }

    private void ToggleButtonsCanvas(bool isShowMenu)
    {
        DOTween.Kill(_moveTween);
        Vector2 pos = isShowMenu ? _hiddenPosiiton: _originalPosition; // show menu -> canvas button hide and revert
        Ease ease = isShowMenu ? Ease.OutQuad : Ease.InQuad;
        _moveTween= _buttonsRectTf.DOAnchorPos(pos,0.3f).SetUpdate(true).SetEase(ease);
    }

}
