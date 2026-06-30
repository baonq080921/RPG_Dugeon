using Base;

namespace Quest
{
    /// <summary>Raised when a scene's quest begins tracking.</summary>
    public struct QuestStartedEvent : IEvent
    {
        public QuestData Quest { get; }
        public QuestStartedEvent(QuestData quest) => Quest = quest;
    }

    /// <summary>Raised each time quest progress increments.</summary>
    public struct QuestProgressEvent : IEvent
    {
        public QuestData Quest { get; }
        public QuestProgressEvent(QuestData quest) => Quest = quest;
    }

    /// <summary>Raised when the quest target is met.</summary>
    public struct QuestCompletedEvent : IEvent
    {
        public QuestData Quest { get; }
        public QuestCompletedEvent(QuestData quest) => Quest = quest;
    }
}
