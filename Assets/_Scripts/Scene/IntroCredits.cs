using System.Collections;
using scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>
/// Plays the intro video and transitions to the main menu when it finishes.
/// </summary>
public class IntroCredits : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;

    // Safety net: on some Android devices the video clip may fail to decode and
    // loopPointReached never fires. Without this fallback the intro hangs forever.
    [SerializeField] private float _maxWaitSeconds = 15f;

    private string _mainMenuScene = "MainMenu";
    private bool _isFinished = false;

    void Awake()
    {
        if (_videoPlayer == null)
            _videoPlayer = GetComponent<VideoPlayer>();

        _videoPlayer.isLooping = false;
        _videoPlayer.playOnAwake = false;
        _videoPlayer.loopPointReached += OnVideoFinished;
        _videoPlayer.errorReceived += OnVideoError;

        // Reason for Prepare instead of Play directly:
        // Android needs the hardware decoder ready before playback. Calling Play()
        // in Awake can fail silently and the finished event would never be raised.
        _videoPlayer.prepareCompleted += OnVideoPrepared;
        _videoPlayer.Prepare();
    }

    void Start() => StartCoroutine(FallbackTimeout());

    private void OnVideoPrepared(VideoPlayer source) => source.Play();

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogError($"[IntroCredits] Video failed to play, skipping intro: {message}");
        LoadMainMenu();
    }

    private void OnVideoFinished(VideoPlayer source) => LoadMainMenu();

    private IEnumerator FallbackTimeout()
    {
        yield return new WaitForSeconds(_maxWaitSeconds);
        if (!_isFinished)
        {
            Debug.LogWarning("[IntroCredits] Intro timed out, loading main menu.");
            LoadMainMenu();
        }
    }

    private void LoadMainMenu()
    {
        if (_isFinished) return;
        _isFinished = true;

        var transition = SceneTransitionManager.Instance;
        if (transition != null)
            transition.FadeToScene(_mainMenuScene);
        else
            SceneManager.LoadScene(_mainMenuScene);
    }
}
