using player;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Connects a <see cref="SkillCooldownUI"/> to one skill slot on the active <see cref="Player"/>.
    /// Set <see cref="_slotIndex"/> in the Inspector to match the slot this button represents.
    /// Place this alongside <see cref="SkillButton"/> and <see cref="SkillCooldownUI"/> on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(SkillCooldownUI))]
    public class SkillCooldownConnector : MonoBehaviour
    {
        [SerializeField] private int _slotIndex;

        private SkillCooldownUI _cooldownUI;
        private Player _currentPlayer;

        private void Awake()
        {
            _cooldownUI = GetComponent<SkillCooldownUI>();
        }

        private void OnEnable()
        {
            Player.ActivePlayerChanged += OnActivePlayerChanged;
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
            UnsubscribeFromPlayer(_currentPlayer);
        }

        private void OnActivePlayerChanged(Player newPlayer)
        {
            UnsubscribeFromPlayer(_currentPlayer);
            _currentPlayer = newPlayer;
            _currentPlayer.SkillManager.SkillCooldownStarted += OnSkillCooldownStarted;
        }

        private void UnsubscribeFromPlayer(Player player)
        {
            if (player != null)
                player.SkillManager.SkillCooldownStarted -= OnSkillCooldownStarted;
        }

        private void OnSkillCooldownStarted(int index, float duration)
        {
            if (index == _slotIndex)
                _cooldownUI.StartCooldown(duration);
        }
    }
}
