using player;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Connects the Player's dash cooldown event to the <see cref="SkillCooldownUI"/>
    /// on this GameObject. Add this alongside <see cref="SkillCooldownUI"/> on the DashButton.
    /// </summary>
    [RequireComponent(typeof(SkillCooldownUI))]
    public class DashCooldownConnector : MonoBehaviour
    {
        [SerializeField] private Player _player;

        private SkillCooldownUI _skillCooldownUI;

        private void Awake()
        {
            _skillCooldownUI = GetComponent<SkillCooldownUI>();
        }

        private void OnEnable()
        {
            _player.DashCooldownStarted += OnDashCooldownStarted;
        }

        private void OnDisable()
        {
            _player.DashCooldownStarted -= OnDashCooldownStarted;
        }

        private void OnDashCooldownStarted(float duration)
        {
            _skillCooldownUI.StartCooldown(duration);
        }
    }
}
