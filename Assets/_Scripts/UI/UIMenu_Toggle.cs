using Base;
using DG.Tweening;
using Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenu_Toggle : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private UIContainerCanvas _uIContainerCanvas;
    [SerializeField] private UIButtonCanvas _uIButtonCanvas;
    
    private Vector2 _hiddenPosiiton = new Vector2(9999f,9999f);
    private Vector2 _originalPosition = Vector2.zero;
    private bool isOpen = false;
    private Tween _moveTween;

    private EventBinding<OnToggleButtonUIEvent> _toggleUiEventBinding;
    public void OnPointerDown(PointerEventData eventData)
    {
        isOpen = !isOpen;
        ToggleUIMenu(isOpen);

    }

    void Awake()
    {
        // FindObjectOfType skips inactive GameObjects by default; pass true so a hidden
        // container/button canvas is still found. Only resolve when not assigned in the
        // Inspector so a manual reference is not overwritten.
        if (_uIButtonCanvas == null)
            _uIButtonCanvas = FindObjectOfType<UIButtonCanvas>(true);
        if (_uIContainerCanvas == null)
            _uIContainerCanvas = FindObjectOfType<UIContainerCanvas>(true);
    }

    void OnEnable()
    {
        _toggleUiEventBinding = new EventBinding<OnToggleButtonUIEvent>(ToggleCanvasButtonEvent);
        EventBus<OnToggleButtonUIEvent>.Register(_toggleUiEventBinding);
    }

    void OnDisable()
    {
        EventBus<OnToggleButtonUIEvent>.Deregister(_toggleUiEventBinding);
    }

    void Start()
    {
        ToggleUIMenu(false);
    }
    private void ToggleUIMenu(bool open)
    {
        RectTransform uiRectTf = _uIContainerCanvas.GetComponent<RectTransform>(); 
        uiRectTf.anchoredPosition = open ? _originalPosition : _hiddenPosiiton;
        ServiceLocator.Get<IGameState>()?.SetPause(open);
        ToggleButtonsCanvas(open);
        EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
    }


    /// <summary>
    /// Call when player open the store or craft store
    /// </summary>
    private void ToggleCanvasButtonEvent(OnToggleButtonUIEvent e)
    {
        ToggleButtonsCanvas(e.isShow);
    }
    private void ToggleButtonsCanvas(bool isShowMenu)
    {
        RectTransform buttonsRectTf = _uIButtonCanvas.GetComponent<RectTransform>();
        DOTween.Kill(_moveTween);
        Vector2 pos = isShowMenu ? _hiddenPosiiton: _originalPosition; // show menu -> canvas button hide and revert
        Ease ease = isShowMenu ? Ease.OutQuad : Ease.InQuad;
        _moveTween= buttonsRectTf.DOAnchorPos(pos,0.3f).SetUpdate(true).SetEase(ease);
    }

}
