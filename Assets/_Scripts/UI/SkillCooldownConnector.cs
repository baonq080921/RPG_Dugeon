using Base;
using player;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Connects a <see cref="SkillCooldownUI"/> to one skill slot on the active <see cref="Player"/>.
    /// Set <see cref="_slotIndex"/> in the Inspector to match the slot this button represents.
    /// Place this alongside <see cref="SkillButton"/> and <see cref="SkillCooldownUI"/> on the same GameObject.
    /// Also drives <see cref="_lockOverlay"/>: visible when the skill is locked, hidden when unlocked.
    /// </summary>
    [RequireComponent(typeof(SkillCooldownUI))]
    public class SkillCooldownConnector : MonoBehaviour
    {
        [SerializeField] private ButtonSkillName _skillName;
        [SerializeField] private GameObject _lockOverlay;

        private SkillCooldownUI _cooldownUI;
        private Player _currentPlayer;

        private void Awake()
        {
            _cooldownUI = GetComponent<SkillCooldownUI>();
        }

        private void OnEnable()
        {
            Player.ActivePlayerChanged += OnActivePlayerChanged;
            UISkillTree.OnReset += SyncOverlay;
            SkillBase.OnAnySkillUnlocked += SyncOverlay;
        }

        private void Start()
        {
            if (_currentPlayer != null) return;
            var existing = FindObjectOfType<Player>();
            if (existing != null)
                OnActivePlayerChanged(existing);
        }

        private void OnDisable()
        {
            Player.ActivePlayerChanged -= OnActivePlayerChanged;
            UISkillTree.OnReset -= SyncOverlay;
            SkillBase.OnAnySkillUnlocked -= SyncOverlay;
            UnsubscribeFromPlayer(_currentPlayer);
        }

        private void OnActivePlayerChanged(Player newPlayer)
        {
            UnsubscribeFromPlayer(_currentPlayer);
            _currentPlayer = newPlayer;
            _currentPlayer.SkillButtonHandler.SkillCooldownStarted += OnSkillCooldownStarted;
            _currentPlayer.SkillButtonHandler.SkillCooldownUpdated += OnSkillCooldownUpdated;
            SyncOverlay();
        }

        private void UnsubscribeFromPlayer(Player player)
        {
            if (player == null) return;
            player.SkillButtonHandler.SkillCooldownStarted -= OnSkillCooldownStarted;
            player.SkillButtonHandler.SkillCooldownUpdated -= OnSkillCooldownUpdated;
        }

        private void SyncOverlay()
        {
            if (_lockOverlay == null || _currentPlayer == null) return;
            var manager = ServiceLocator.Get<PlayerSkillManager>();
            if (manager == null) return;

            SkillBase skill = _skillName == ButtonSkillName.CounterSkill
                ? manager.skillCounter
                : manager.GetSkillByType(ToSkillType(_skillName));

            _lockOverlay.SetActive(skill == null || !skill.IsUnlocked);
        }

        private static SkillType ToSkillType(ButtonSkillName name) => name switch
        {
            ButtonSkillName.Dash       => SkillType.Dash,
            ButtonSkillName.TimeEcho   => SkillType.TimeEcho,
            ButtonSkillName.Dismantle  => SkillType.Dismantle,
            ButtonSkillName.Domain     => SkillType.Domain,
            _                          => SkillType.Dash
        };

        private void OnSkillCooldownStarted(int index, float duration)
        {
            if (index == (int)_skillName)
                _cooldownUI.StartCooldown(duration);
        }

        private void OnSkillCooldownUpdated(int index, float remaining)
        {
            if (index == (int)_skillName)
                _cooldownUI.StartCooldown(remaining);
        }
    }
}
