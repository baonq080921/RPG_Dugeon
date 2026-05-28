using System;
using UnityEngine;

namespace player
{
    /// <summary>
    /// Manages all player skill slots: cooldowns, pending button presses, and state lookups.
    /// Skill states are registered at runtime by <see cref="Player.CreateStates"/>.
    /// On-screen buttons call <see cref="PressSkill"/> to queue a skill for the next state update.
    /// </summary>
    public class SkillButtonHandler : MonoBehaviour
    {
        [SerializeField] private SkillBaseDefinition[] _skillDefinition;

        private PlayerState[] _states;
        private float[] _cooldownTimers;
        private bool[] _pending;

        /// <summary>
        /// Fired when a skill enters cooldown. Arguments: slot index, cooldown duration in seconds.
        /// </summary>
        public event Action<int, float> SkillCooldownStarted;

        /// <summary>Number of configured skill slots.</summary>
        public int SlotCount => _skillDefinition?.Length ?? 0;

        private void Update()
        {
            if (_cooldownTimers == null) return;
            for (int i = 0; i < _cooldownTimers.Length; i++)
            {
                if (_cooldownTimers[i] > 0f)
                    _cooldownTimers[i] -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Binds a <see cref="PlayerState"/> to the given slot index.
        /// Call once per skill in <see cref="Player.CreateStates"/>.
        /// </summary>
        public void RegisterState(int index, PlayerState state)
        {
            EnsureInitialized();
            if (index < 0 || index >= SlotCount) return;
            _states[index] = state;
        }

        /// <summary>
        /// Queues the skill at <paramref name="index"/> to activate on the next state update.
        /// Silently ignored when the skill is on cooldown or the index is out of range.
        /// Called by on-screen <see cref="UI.SkillButton"/> components.
        /// </summary>
        public void PressSkill(ButtonSkillName skillName)
        {
            int index = (int)skillName;
            EnsureInitialized();
            if (index < 0 || index >= SlotCount) return;
            if (!IsReady(index)) return;
            _pending[index] = true;
        }

        /// <summary>
        /// If the skill at <paramref name="index"/> is pending and ready, consumes the press,
        /// starts the cooldown, and outputs the <see cref="PlayerState"/> to enter.
        /// </summary>
        /// <returns>True when a skill was consumed and <paramref name="state"/> is valid.</returns>
        public bool TryConsumeSkill(int index, out PlayerState state)
        {
            state = null;
            if (_pending == null) return false;
            if (index < 0 || index >= SlotCount) return false;
            if (!_pending[index] || _states[index] == null) return false;

            _pending[index] = false;
            float duration = _skillDefinition[index].Cooldown;
            _cooldownTimers[index] = duration;
            SkillCooldownStarted?.Invoke(index, duration);
            state = _states[index];
            return true;
        }

        /// <returns>The <see cref="SkillDefinition"/> for the given slot, or null if out of range.</returns>
        public SkillBaseDefinition GetSkill(int index) =>
            (index >= 0 && index < SlotCount) ? _skillDefinition[index] : null;

        /// <returns>True if the skill at <paramref name="index"/> has no active cooldown.</returns>
        public bool IsReady(int index)
        {
            if (_cooldownTimers == null) return true;
            return index >= 0 && index < SlotCount && _cooldownTimers[index] <= 0f;
        }

        private void EnsureInitialized()
        {
            if (_states != null) return;
            int count = SlotCount;
            _states = new PlayerState[count];
            _cooldownTimers = new float[count];
            _pending = new bool[count];
        }
    }
}
