using Base;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Shown automatically when <see cref="PlayerDiedEvent"/> is raised.
    /// Wires three buttons: exit to main menu, restore from last checkpoint, and start a new game.
    /// </summary>
    public class UIGameOver : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _backToCheckpointButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private string _mainMenuSceneName = "MainMenu";
        [SerializeField] private string _firstSceneName = "Room0";

        private SaveManager _saveManager;
        private EventBinding<PlayerDiedEvent> _diedBinding;

        private void Start()
        {
            _saveManager = ServiceLocator.Get<SaveManager>();
            _panel.SetActive(false);

            _exitButton.onClick.AddListener(OnExitClicked);
            _backToCheckpointButton.onClick.AddListener(OnBackToCheckpointClicked);
            _newGameButton.onClick.AddListener(OnNewGameClicked);

            _backToCheckpointButton.interactable = _saveManager != null && _saveManager.HasSave();
        }

        private void OnEnable()
        {
            _diedBinding = new EventBinding<PlayerDiedEvent>(ShowPanel);
            EventBus<PlayerDiedEvent>.Register(_diedBinding);
        }

        private void OnDisable()
        {
            EventBus<PlayerDiedEvent>.Deregister(_diedBinding);
        }

        private void ShowPanel()
        {
            _panel.SetActive(true);
            ServiceLocator.Get<GameManager>()?.SetPause(true);
        }
        private void HidePanel()
        {
            _panel.SetActive(false);
            ServiceLocator.Get<GameManager>()?.SetPause(false);

        }
        /// <summary>Assign to the Exit button OnClick event.</summary>
        public void OnExitClicked()
        {
            HidePanel();
            SceneManager.LoadScene(_mainMenuSceneName);
        }

        /// <summary>Assign to the Back To Checkpoint button OnClick event.</summary>
        public void OnBackToCheckpointClicked()
        {
            if (_saveManager == null || !_saveManager.HasSave()) return;
            HidePanel();
            _saveManager.Load();
        }

        /// <summary>Assign to the New Game button OnClick event.</summary>
        public void OnNewGameClicked()
        {
            if (_saveManager == null) return;
            HidePanel();
            _saveManager.StartNewGame(_firstSceneName);
        }
    }
}
