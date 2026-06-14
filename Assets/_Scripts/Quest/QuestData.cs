using System;
using UnityEngine;

namespace Quest
{
    /// <summary>
    /// Base ScriptableObject for all quest types.
    /// Subclasses define what the target is by implementing <see cref="StartTracking"/>,
    /// <see cref="StopTracking"/>, <see cref="IsCompleted"/>, and <see cref="ProgressText"/>.
    /// Runtime progress is stored in [NonSerialized] fields on the subclass so the
    /// asset itself is never modified during play.
    /// </summary>
    public abstract class QuestData : ScriptableObject
    {
        [field: SerializeField] public string QuestName { get; private set; }
        [field: SerializeField, TextArea] public string QuestDescription { get; private set; }

        public abstract bool IsCompleted { get; }
        public abstract string ProgressText { get; }

        public event Action OnProgressChanged;
        public event Action OnQuestCompleted;

        /// <summary>Subscribe to whatever game events this quest tracks.</summary>
        public abstract void StartTracking();

        /// <summary>Unsubscribe from all tracked events.</summary>
        public abstract void StopTracking();

        /// <summary>Reset runtime progress counters back to zero.</summary>
        public abstract void Reset();

        protected void NotifyProgress() => OnProgressChanged?.Invoke();
        protected void NotifyCompleted() => OnQuestCompleted?.Invoke();
    }
}
