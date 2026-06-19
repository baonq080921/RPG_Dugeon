using Base;
using UnityEngine;
using UnityEngine.UI;
public class UI_Shop : MonoBehaviour
{
    private EventBinding<StoreCallEvent> _storeCallEventBinding;
    private Vector2 _originalPosition = Vector2.zero;
    private Vector2 _hiddentPosition = new Vector2(9000f, 9000f);
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _closeBtn;
    void Start()
    {
        _rectTransform.anchoredPosition = _hiddentPosition;
    }

    void OnEnable()
    {
        _storeCallEventBinding = new EventBinding<StoreCallEvent>(OpenStoreUI);
        EventBus<StoreCallEvent>.Register(_storeCallEventBinding);
        _closeBtn.onClick.AddListener(CloseStoreUI);
    }

    void OnDisable()
    {
        EventBus<StoreCallEvent>.Deregister(_storeCallEventBinding);
        _closeBtn.onClick.RemoveListener(CloseStoreUI);
    }


    private void OpenStoreUI() => _rectTransform.anchoredPosition = _originalPosition;
    public void CloseStoreUI() => _rectTransform.anchoredPosition = _hiddentPosition;
}
