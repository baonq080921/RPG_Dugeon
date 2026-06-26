using Base;
using Save;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Wires the Main Menu New Game / Continue buttons to <see cref="SaveManager"/>.
    /// Attach to the Canvas root (or any active GameObject) in the MainMenu scene.
    /// Assign the three buttons and the first gameplay scene name in the Inspector.
    /// </summary>
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private string _firstSceneName = "Room1";

        private SaveManager _saveManager;

        private void Start()
        {
            _saveManager = ServiceLocator.Get<SaveManager>();

            // Grey out Continue if there is nothing saved yet.
            if (_continueButton != null)
                _continueButton.interactable = _saveManager != null && _saveManager.HasSave();
        }

        /// <summary>Assign to the New Game button OnClick event.</summary>
        public void OnNewGameClicked()
        {
            if (_saveManager == null) return;
            _saveManager.StartNewGame(_firstSceneName);
        }

        /// <summary>Assign to the Continue button OnClick event.</summary>
        public void OnContinueClicked()
        {
            if (_saveManager == null || !_saveManager.HasSave()) return;
            _saveManager.Load();
        }
    }
}
