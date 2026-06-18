using Base;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quest
{
    /// <summary>
    /// Place one of these in each scene that has a quest.
    /// Starts tracking on <see cref="Start"/>, checks the save file so already-completed
    /// quests are never re-tracked, and saves completion back to disk when done.
    /// </summary>
    public class SceneQuestController : MonoBehaviour
    {
        [SerializeField] private QuestData _quest;

        public QuestData Quest => _quest;
        public bool IsQuestComplete { get; private set; }

        private void Start()
        {
            if (_quest == null) return;

            var saveManager = ServiceLocator.Get<SaveManager>();
            string sceneName = SceneManager.GetActiveScene().name;

            if (saveManager != null && saveManager.IsQuestComplete(sceneName))
            {
                IsQuestComplete = true;
                EventBus<QuestCompletedEvent>.Raise(new QuestCompletedEvent(_quest));
                return;
            }

            _quest.Reset();

            int savedProgress = saveManager?.GetQuestProgress(sceneName) ?? 0;
            if (savedProgress > 0)
                _quest.RestoreProgress(savedProgress);

            _quest.OnProgressChanged += OnProgress;
            _quest.OnQuestCompleted  += OnComplete;
            _quest.StartTracking();

            EventBus<QuestStartedEvent>.Raise(new QuestStartedEvent(_quest));
        }

        private void OnDestroy()
        {
            if (_quest == null) return;
            _quest.StopTracking();
            _quest.OnProgressChanged -= OnProgress;
            _quest.OnQuestCompleted  -= OnComplete;
        }

        private void OnProgress()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            ServiceLocator.Get<SaveManager>()?.SetQuestProgress(sceneName, _quest.CurrentProgress);
            EventBus<QuestProgressEvent>.Raise(new QuestProgressEvent(_quest));
        }

        private void OnComplete()
        {
            IsQuestComplete = true;
            _quest.StopTracking();
            _quest.OnProgressChanged -= OnProgress;
            _quest.OnQuestCompleted  -= OnComplete;

            string sceneName = SceneManager.GetActiveScene().name;
            ServiceLocator.Get<SaveManager>()?.MarkQuestComplete(sceneName);

            EventBus<QuestCompletedEvent>.Raise(new QuestCompletedEvent(_quest));
        }
    }
}
