using System;

namespace Save
{
    /// <summary>Persists the numeric progress of a single in-progress quest, keyed by scene name.</summary>
    [Serializable]
    public class QuestProgressEntry
    {
        public string sceneName;
        public int    progress;
    }
}
