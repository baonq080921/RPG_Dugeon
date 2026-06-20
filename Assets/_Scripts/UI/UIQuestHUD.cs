using Base;
using DG.Tweening;
using Quest;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Displays quest info inside the Quest tab panel (opened via button, like Inventory/SkillTree).
    /// The panel itself is controlled by <see cref="UICanvasChange"/> — this script only
    /// keeps the text fields up to date and auto-pops the Mission Complete banner.
    /// </summary>
    public class UIQuestHUD : MonoBehaviour
    {
        [Header("Quest Info (inside the tab panel)")]
        [SerializeField] private TextMeshProUGUI _questNameTmp;
        [SerializeField] private TextMeshProUGUI _questDescriptionTmp;
        [SerializeField] private TextMeshProUGUI _progressTmp;
        [SerializeField] private TextMeshProUGUI _noQuestTmp;

        [Header("Mission Complete Banner (auto-pops on completion)")]
        [SerializeField] private CanvasGroup _completeBanner;
        [SerializeField] private float _bannerDuration = 2.5f;

        private EventBinding<QuestStartedEvent>   _startedBinding;
        private EventBinding<QuestProgressEvent>  _progressBinding;
        private EventBinding<QuestCompletedEvent> _completedBinding;

        private void OnEnable()
        {
            _startedBinding   = new EventBinding<QuestStartedEvent>(OnQuestStarted);
            _progressBinding  = new EventBinding<QuestProgressEvent>(OnQuestProgress);
            _completedBinding = new EventBinding<QuestCompletedEvent>(OnQuestCompleted);

            EventBus<QuestStartedEvent>.Register(_startedBinding);
            EventBus<QuestProgressEvent>.Register(_progressBinding);
            EventBus<QuestCompletedEvent>.Register(_completedBinding);
        }

        private void OnDisable()
        {
            EventBus<QuestStartedEvent>.Deregister(_startedBinding);
            EventBus<QuestProgressEvent>.Deregister(_progressBinding);
            EventBus<QuestCompletedEvent>.Deregister(_completedBinding);
        }

        private void Start()
        {
            ShowNoQuestState();
            if (_completeBanner != null)
            {
                _completeBanner.alpha = 0f;
                _completeBanner.gameObject.SetActive(false);
            }
        }

        private void OnQuestStarted(QuestStartedEvent e)
        {
            var quest = e.Quest;
            if (_noQuestTmp != null) _noQuestTmp.gameObject.SetActive(false);
            ShowQuestDetailInfo(quest, true);
        }

        private void ShowQuestDetailInfo(QuestData data, bool isStart)
        {
            _progressTmp.color = isStart ? Color.red: Color.yellow;            
            if (_questNameTmp != null) _questNameTmp.text = data.QuestName;
            if (_questDescriptionTmp != null) _questDescriptionTmp.text = data.QuestDescription;
            if (_progressTmp != null) _progressTmp.text = data.ProgressText;
        }

        private void OnQuestProgress(QuestProgressEvent e)
        {
            var quest = e.Quest;
            if (_progressTmp != null) ShowQuestDetailInfo(quest,false);
            
                
        }

        private void OnQuestCompleted(QuestCompletedEvent e)
        {
            var quest = e.Quest;
            if (_progressTmp != null) _progressTmp.text = quest.ProgressText;
            ShowCompleteBanner();
            ShowCompleteText();
        }

        private void ShowCompleteText()
        {
            ShowNoQuestState();
            _noQuestTmp.text = "You Finish Objective for this scene .Not things else to do here";
        }

        

        private void ShowCompleteBanner()
        {
            if (_completeBanner == null) return;
            _completeBanner.DOKill();
            _completeBanner.alpha = 0f;
            _completeBanner.gameObject.SetActive(true);

            DOTween.Sequence()
                .Append(_completeBanner.DOFade(1f, 0.4f))
                .AppendInterval(_bannerDuration)
                .Append(_completeBanner.DOFade(0f, 0.4f))
                .OnComplete(() => _completeBanner.gameObject.SetActive(false))
                .SetUpdate(true);
        }

        private void ShowNoQuestState()
        {
            if (_noQuestTmp != null) _noQuestTmp.gameObject.SetActive(true);
            if (_questNameTmp != null)        _questNameTmp.text        = string.Empty;
            if (_questDescriptionTmp != null) _questDescriptionTmp.text = string.Empty;
            if (_progressTmp != null)         _progressTmp.text         = string.Empty;
        }
    }
}
