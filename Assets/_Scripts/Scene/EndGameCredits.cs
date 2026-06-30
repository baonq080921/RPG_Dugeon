using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace scene
{
    /// <summary>
    /// Plays the end-game credits video in the EndGame scene, then loads the main menu.
    /// Renders the video to a screen-sized RenderTexture shown on a full-screen RawImage,
    /// so it works without a scene camera (the gameplay camera is a persistent object).
    /// If no video is assigned it falls back to a fixed wait so the flow still completes.
    /// The credits can optionally be skipped by tapping the screen.
    /// </summary>
    public class EndGameCredits : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;

        [SerializeField] private string _mainMenuSceneName = "MainMenu";
        [Header("Flow")]
        [Tooltip("Seconds to wait before loading the menu when no VideoPlayer/clip is assigned.")]
        [SerializeField] private float _fallbackDuration = 10f;
        [Tooltip("Allow the player to tap the screen to skip the credits.")]
        [SerializeField] private bool _allowSkip = true;

        private RenderTexture _runtimeRenderTexture;
        private bool _isFinished;

        private void Start()
        {
            // Boss death may leave the game paused; make sure time runs in the credits scene.
            Time.timeScale = 1f;

            if (_videoPlayer == null)
                _videoPlayer = GetComponent<VideoPlayer>();

            if (_videoPlayer == null)
            {
                // No video assigned: wait a fixed duration then continue.
                StartCoroutine(FallbackWait());
                return;
            }
            _videoPlayer.isLooping = false;
            _videoPlayer.loopPointReached += OnVideoFinished;
            _videoPlayer.Play();
        }



        private void Update()
        {
            if (!_allowSkip || _isFinished) return;
            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
                LoadMainMenu();
        }

        private void OnDisable()
        {
            if (_videoPlayer != null)
                _videoPlayer.loopPointReached -= OnVideoFinished;
        }

        private void OnDestroy()
        {
            if (_runtimeRenderTexture == null) return;
            _runtimeRenderTexture.Release();
            Destroy(_runtimeRenderTexture);
        }

        /// <summary>Invoked by the <see cref="VideoPlayer"/> when the clip reaches its end.</summary>
        private void OnVideoFinished(VideoPlayer source) => LoadMainMenu();

        private IEnumerator FallbackWait()
        {
            yield return new WaitForSeconds(_fallbackDuration);
            LoadMainMenu();
        }

        private void LoadMainMenu()
        {
            if (_isFinished) return;
            _isFinished = true;
            SceneManager.LoadScene(_mainMenuSceneName);
        }
    }
}
