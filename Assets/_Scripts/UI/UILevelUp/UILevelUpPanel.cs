using Base;
using DG.Tweening;
using Interfaces;
using player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Shown whenever <see cref="PlayerLevelUpEvent"/> is raised.
    /// Pauses the game, fades the panel in, and lets the player pick one major stat to permanently increase.
    /// Uses DOTween fade + position-based hiding instead of SetActive.
    /// </summary>
    public class UILevelUpPanel : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private RectTransform _panelRect;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Stat Buttons")]
        [SerializeField] private Button _strengthButton;
        [SerializeField] private Button _agilityButton;
        [SerializeField] private Button _intelligenceButton;
        [SerializeField] private Button _vitalityButton;

        private readonly Vector2 _hiddenPosition = new Vector2(9000f, 9000f);
        private Vector2 _originalPosition;

        private EntityStat _pendingStat;
        private player.PlayerLevel _playerLevel;
        private EventBinding<PlayerLevelUpEvent> _levelUpBinding;

        private void Awake()
        {
            _originalPosition = _panelRect.anchoredPosition;
            _panelRect.anchoredPosition = _hiddenPosition;
            _canvasGroup.alpha = 0f;
            _playerLevel = FindObjectOfType<player.PlayerLevel>();

            _strengthButton.onClick.AddListener(()     => ChooseStat(StatType.Strength));
            _agilityButton.onClick.AddListener(()      => ChooseStat(StatType.Agility));
            _intelligenceButton.onClick.AddListener(() => ChooseStat(StatType.Intelligence));
            _vitalityButton.onClick.AddListener(()     => ChooseStat(StatType.Vitality));
        }

        private void OnEnable()
        {
            _levelUpBinding = new EventBinding<PlayerLevelUpEvent>(OnLevelUp);
            EventBus<PlayerLevelUpEvent>.Register(_levelUpBinding);
        }

        private void OnDisable()
        {
            EventBus<PlayerLevelUpEvent>.Deregister(_levelUpBinding);
        }

        private void OnLevelUp(PlayerLevelUpEvent e)
        {
            _pendingStat = e.PlayerStat;

            if (_levelText != null && _playerLevel != null)
                _levelText.text = $"Level {_playerLevel.Level}!\nChoose a stat to upgrade";

            ServiceLocator.Get<IGameState>().SetPause(true);

            _panelRect.anchoredPosition = _originalPosition;
            DOTween.Kill(_canvasGroup);
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.DOFade(1f, 1f).SetUpdate(true).SetEase(Ease.InQuad);
            EventBus<OnToggleButtonUIEvent>.Raise(new OnToggleButtonUIEvent(true));
        }

        private void ChooseStat(StatType statType)
        {
            _pendingStat?.AddMajorStatPoint(statType);

            DOTween.Kill(_canvasGroup);
            _canvasGroup.DOFade(0f, 1f).SetUpdate(true).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _canvasGroup.blocksRaycasts = false;
                    _panelRect.anchoredPosition = _hiddenPosition;
                    ServiceLocator.Get<IGameState>().SetPause(false);
                    EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());
                        EventBus<OnToggleButtonUIEvent>.Raise(new OnToggleButtonUIEvent(true));
                });
        }
    }
}
