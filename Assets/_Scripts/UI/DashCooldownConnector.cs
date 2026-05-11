using player;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Automatically hooks into whichever <see cref="Player"/> is currently active.
    /// Listens to <see cref="Player.ActivePlayerChanged"/> so it works with character
    /// switching without any manual Inspector wiring.
    /// </summary>
    [RequireComponent(typeof(SkillCooldownUI))]
    public class DashCooldownConnector : MonoBehaviour
    {
        private SkillCooldownUI _skillCooldownUI;
        private Player _currentPlayer;

        private void Awake()
        {
            _skillCooldownUI = GetComponent<SkillCooldownUI>();
        }

        private void OnEnable()
        {
            Player.ActivePlayerChanged += OnActivePlayerChanged;
            // Player.OnEnable may have already fired before we subscribed, grab it directly
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
            _currentPlayer.DashCooldownStarted += OnDashCooldownStarted;
        }

        private void UnsubscribeFromPlayer(Player player)
        {
            if (player != null)
                player.DashCooldownStarted -= OnDashCooldownStarted;
        }

        private void OnDashCooldownStarted(float duration)
        {
            _skillCooldownUI.StartCooldown(duration);
        }
    }
}