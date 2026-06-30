using Base;
using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private ISaveService _saveService;
    private EventBinding<PlayerDiedEvent> _diedBinding;

    private void Start()
    {
        _saveService = ServiceLocator.Get<ISaveService>();
        _panel.SetActive(false);

        _exitButton.onClick.AddListener(OnExitClicked);
        _backToCheckpointButton.onClick.AddListener(OnBackToCheckpointClicked);
        _newGameButton.onClick.AddListener(OnNewGameClicked);
        _backToCheckpointButton.interactable = _saveService != null && _saveService.HasSave();
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
        _backToCheckpointButton.interactable = _saveService != null && _saveService.HasSave(); // recheck if player can backToCheckPoint
        ServiceLocator.Get<IGameState>()?.SetPause(true);

    }
    private void HidePanel()
    {
        _panel.SetActive(false);
        ServiceLocator.Get<IGameState>()?.SetPause(false);

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
        if (_saveService == null || !_saveService.HasSave()) return;
        HidePanel();
        _saveService.Load();
    }

    /// <summary>Assign to the New Game button OnClick event.</summary>
    public void OnNewGameClicked()
    {
        if (_saveService == null) return;
            HidePanel();
        _saveService.StartNewGame(_firstSceneName);
    }
}
