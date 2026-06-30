using Base;
using Interfaces;
using player;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
public class UI_Shop : MonoBehaviour
{
    private EventBinding<StoreCallEvent> _storeCallEventBinding;
    private Vector2 _originalPosition = Vector2.zero;
    private Vector2 _hiddentPosition = new Vector2(9000f, 9000f);
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private TextMeshProUGUI _playerMoneyText;
    private PlayerInventory _playerInventory;
    void Start()
    {
        _rectTransform.anchoredPosition = _hiddentPosition;
    }

    void OnEnable()
    {
        _storeCallEventBinding = new EventBinding<StoreCallEvent>(OpenStoreUI);
        EventBus<StoreCallEvent>.Register(_storeCallEventBinding);
        _closeBtn.onClick.AddListener(CloseStoreUI);
        Player.ActivePlayerChanged += OnPlayerChanged;

        if (Player.ActivePlayer != null)
            OnPlayerChanged(Player.ActivePlayer);
    }

    void OnDisable()
    {
        EventBus<StoreCallEvent>.Deregister(_storeCallEventBinding);
        _closeBtn.onClick.RemoveListener(CloseStoreUI);
        Player.ActivePlayerChanged -= OnPlayerChanged;
    }


    private void OnPlayerChanged(Player player)
    {
        if (_playerInventory != null)
            _playerInventory.OnMoneyChanged -= UpdateDisplay;
        _playerInventory = player.playerInventory;
        _playerInventory.OnMoneyChanged += UpdateDisplay;
        UpdateDisplay(_playerInventory.Money);
    }

    private void UpdateDisplay(float amount)
    {
        _playerMoneyText.text = BuildText(amount);    
    }
    private string BuildText(float amount)
    {
        return $"Your gold: <color=green>{amount}</color>";
    }

    private void OpenStoreUI()
    {
        UpdateDisplay(_playerInventory.Money);
        _rectTransform.anchoredPosition = _originalPosition;
        EventBus<OnToggleButtonUIEvent>.Raise(new OnToggleButtonUIEvent(true));
        ServiceLocator.Get<IGameState>().SetPause(true);
    }
    public void CloseStoreUI()
    {
        _rectTransform.anchoredPosition = _hiddentPosition;
        EventBus<OnToggleButtonUIEvent>.Raise(new OnToggleButtonUIEvent(false));
        ServiceLocator.Get<IGameState>().SetPause(false);
    }
}
