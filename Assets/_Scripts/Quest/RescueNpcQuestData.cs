using System;
using Base;
using UnityEngine;

namespace Quest
{
    /// <summary>
    /// Quest that completes when the player rescues <see cref="RequiredRescueCount"/> NPCs.
    /// Place <see cref="NPC.RescuableNpc"/> on each NPC in the scene that should count.
    /// </summary>
    [CreateAssetMenu(fileName = "RescueNpcQuest", menuName = "RPG/Quest/Rescue NPC Quest")]
    public class RescueNpcQuestData : QuestData
    {
        [field: SerializeField] public int RequiredRescueCount { get; private set; } = 3;

        [NonSerialized] private int _rescuedCount;
        private EventBinding<NpcRescuedEvent> _binding;

        public override bool IsCompleted => _rescuedCount >= RequiredRescueCount;
        public override string ProgressText => $"{_rescuedCount} / {RequiredRescueCount}";

        /// <inheritdoc/>
        public override void StartTracking()
        {
            _binding = new EventBinding<NpcRescuedEvent>(OnNpcRescued);
            EventBus<NpcRescuedEvent>.Register(_binding);
        }

        /// <inheritdoc/>
        public override void StopTracking()
        {
            if (_binding != null)
                EventBus<NpcRescuedEvent>.Deregister(_binding);
        }

        /// <inheritdoc/>
        public override void Reset() => _rescuedCount = 0;

        private void OnNpcRescued(NpcRescuedEvent e)
        {
            if (IsCompleted) return;
            _rescuedCount++;
            NotifyProgress();
            if (IsCompleted)
                NotifyCompleted();
        }
    }
}
