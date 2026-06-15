using System.Collections;
using Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace scene
{
    /// <summary>
    /// Singleton that handles scene transitions with a black-screen fade.
    /// Persists across scenes and places the player at the correct spawn point after loading.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [SerializeField] private float _fadeDuration = 0.5f;

        private CanvasGroup _fadeCanvasGroup;
        private string _pendingDestinationPortalId;

        /// <summary>True while a scene transition is in progress.</summary>
        public bool IsTransitioning { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupFadeCanvas();
        }

        /// <summary>
        /// Begins a fade-out, loads <paramref name="sceneName"/>, places the player at the
        /// spawn point matching <paramref name="destinationPortalId"/>, then fades back in.
        /// </summary>
        public void TransitionToScene(string sceneName, string destinationPortalId)
        {
            if (IsTransitioning) return;
            _pendingDestinationPortalId = destinationPortalId;
            StartCoroutine(TransitionRoutine(sceneName));
        }

        private IEnumerator TransitionRoutine(string sceneName)
        {
            IsTransitioning = true;
            _fadeCanvasGroup.blocksRaycasts = true;

            yield return StartCoroutine(Fade(1f));

            // Snapshot before the old scene is destroyed
            var saveManager = ServiceLocator.Get<Save.SaveManager>();
            var leavingPlayer = FindObjectOfType<player.Player>();
            if (leavingPlayer != null && saveManager != null)
                saveManager.SnapshotForTransition(leavingPlayer);

            yield return SceneManager.LoadSceneAsync(sceneName);

            // Position first, then restore all other state
            MovePlayerToSpawn();
            var arrivingPlayer = FindObjectOfType<player.Player>();
            if (arrivingPlayer != null && saveManager != null && saveManager.HasTransitionSnapshot)
                yield return StartCoroutine(saveManager.RestoreAfterTransition(arrivingPlayer));

            yield return StartCoroutine(Fade(0f));

            _fadeCanvasGroup.blocksRaycasts = false;
            IsTransitioning = false;
        }

        private IEnumerator Fade(float targetAlpha)
        {
            float startAlpha = _fadeCanvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);
                yield return null;
            }
            _fadeCanvasGroup.alpha = targetAlpha;
        }

        private void MovePlayerToSpawn()
        {
            if (string.IsNullOrEmpty(_pendingDestinationPortalId)) return;

            var player = FindObjectOfType<player.Player>();
            if (player == null) return;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;

            foreach (var sp in FindObjectsOfType<SceneSpawnPoint>(true))
            {
                if (sp.PortalId != _pendingDestinationPortalId) continue;
                player.input.Disable();
                player.transform.DOMove(sp.transform.position, 0.5f).SetUpdate(true).OnComplete(() =>
                {
                    player.input.Enable();
                });
                break;
            }
            _pendingDestinationPortalId = null;
        }

        private void SetupFadeCanvas()
        {
            var canvasGo = new GameObject("TransitionFadeCanvas");
            canvasGo.transform.SetParent(transform);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            canvasGo.AddComponent<CanvasScaler>();

            _fadeCanvasGroup = canvasGo.AddComponent<CanvasGroup>();
            _fadeCanvasGroup.alpha = 0f;
            _fadeCanvasGroup.blocksRaycasts = false;
            _fadeCanvasGroup.interactable = false;

            var panelGo = new GameObject("FadePanel");
            panelGo.transform.SetParent(canvasGo.transform, false);
            panelGo.AddComponent<Image>().color = Color.black;
            var rt = panelGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
