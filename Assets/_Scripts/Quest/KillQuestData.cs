using System;
using Base;
using UnityEngine;

namespace Quest
{
    /// <summary>
    /// Quest that completes when the player kills <see cref="RequiredKillCount"/> enemies.
    /// Hooks into the existing <see cref="EnemyDiedEvent"/> on the EventBus.
    /// </summary>
    [CreateAssetMenu(fileName = "KillQuest", menuName = "RPG/Quest/Kill Quest")]
    public class KillQuestData : QuestData
    {
        [field: SerializeField] public int RequiredKillCount { get; private set; } = 5;

        [NonSerialized] private int _currentKillCount;
        private EventBinding<EnemyDiedEvent> _binding;

        public override bool IsCompleted => _currentKillCount >= RequiredKillCount;
        public override string ProgressText => $"{_currentKillCount} / {RequiredKillCount}";

        /// <inheritdoc/>
        public override void StartTracking()
        {
            _binding = new EventBinding<EnemyDiedEvent>(OnEnemyDied);
            EventBus<EnemyDiedEvent>.Register(_binding);
        }

        /// <inheritdoc/>
        public override void StopTracking()
        {
            if (_binding != null)
                EventBus<EnemyDiedEvent>.Deregister(_binding);
        }

        /// <inheritdoc/>
        public override void Reset() => _currentKillCount = 0;

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            if (IsCompleted) return;
            _currentKillCount++;
            NotifyProgress();
            if (IsCompleted)
                NotifyCompleted();
        }
    }
}
