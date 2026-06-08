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
        private SkillBase[] _skills;
        private float[] _cooldownTimers;
        private bool[] _pending;

        /// <summary>Fired when a skill enters cooldown. Arguments: slot index, full cooldown duration.</summary>
        public event Action<int, float> SkillCooldownStarted;

        /// <summary>Fired when a cooldown is externally shortened. Arguments: slot index, new remaining seconds.</summary>
        public event Action<int, float> SkillCooldownUpdated;

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
        /// Binds a <see cref="SkillBase"/> to the given slot index so <see cref="PressSkill"/>
        /// can gate activation on <see cref="SkillBase.IsUnlocked"/>.
        /// Call once per skill in <see cref="Player"/>'s Start.
        /// </summary>
        public void RegisterSkill(int index, SkillBase skill)
        {
            EnsureInitialized();
            if (index < 0 || index >= SlotCount) return;
            _skills[index] = skill;
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
            if (_skills[index] != null && !_skills[index].IsUnlocked) return;
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
            float cooldown = _skillDefinition[index].Cooldown;
            _cooldownTimers[index] = cooldown;
            SkillCooldownStarted?.Invoke(index, cooldown);
            state = _states[index];
            return true;
        }

        /// <summary>
        /// Consumes a queued skill that has no associated <see cref="PlayerState"/> (e.g. instant-effect skills).
        /// Starts the cooldown and fires <see cref="SkillCooldownStarted"/>.
        /// </summary>
        /// <returns>True when the skill was pending and ready to fire.</returns>
        public bool TryConsumeEffect(int index)
        {
            if (_pending == null) return false;
            if (index < 0 || index >= SlotCount) return false;
            if (!_pending[index]) return false;

            _pending[index] = false;
            float duration = _skillDefinition[index].Cooldown;
            _cooldownTimers[index] = duration;
            SkillCooldownStarted?.Invoke(index, duration);
            return true;
        }

        /// <returns>The <see cref="SkillDefinition"/> for the given slot, or null if out of range.</returns>
        public SkillBaseDefinition GetSkill(int index) =>
            (index >= 0 && index < SlotCount) ? _skillDefinition[index] : null;

        /// <summary>
        /// Reduces the remaining cooldown of every skill that is currently on cooldown
        /// by <paramref name="reductionPercent"/> of its remaining time.
        /// Example: 0.5 halves all active cooldowns.
        /// </summary>
        public void ReduceAllCooldowns(float reductionPercent)
        {
            if (_cooldownTimers == null) return;
            float multiplier = 1f - Mathf.Clamp01(reductionPercent);
            for (int i = 0; i < _cooldownTimers.Length; i++)
            {
                if (_cooldownTimers[i] <= 0f) continue;
                _cooldownTimers[i] *= multiplier;
                SkillCooldownUpdated?.Invoke(i, _cooldownTimers[i]);
            }
        }

        public void ReduceSkillCoolDown(float reductionPercent, int skillIndex)
        {
            if (_cooldownTimers == null) return;
            if (skillIndex < 0 || skillIndex >= _cooldownTimers.Length) return;
            if (_cooldownTimers[skillIndex] <= 0f) return;
            _cooldownTimers[skillIndex] *= 1f - Mathf.Clamp01(reductionPercent);
            SkillCooldownUpdated?.Invoke(skillIndex, _cooldownTimers[skillIndex]);
        }

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
            _skills = new SkillBase[count];
            _cooldownTimers = new float[count];
            _pending = new bool[count];
        }
    }
}
