using System.Collections;
using Cinemachine;
using player;
using scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
    /// <summary>
    /// Adjusts the Cinemachine Virtual Camera orthographic size once at startup so the
    /// horizontal world extent is always consistent with a 16:9 landscape reference,
    /// regardless of the device's actual screen ratio.
    /// Also reassigns the <see cref="CinemachineConfiner"/> bounding shape on every scene
    /// load by finding the scene's <see cref="CameraConfinerBounds"/> object.
    /// </summary>
    public class CameraResolutionFitter : MonoBehaviour
    {
        private const float TargetAspect = 16f / 9f;

        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineConfiner2D _confiner;
        private Transform _pendingFollowTarget;
        private bool _aspectApplied;

        private EventBinding<CameraZoneChangedEvent> _zoneBinding;

        private void OnEnable()
        {
            Player.ActivePlayerChanged += OnPlayerActive;
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (Player.ActivePlayer != null)
                OnPlayerActive(Player.ActivePlayer);
            _zoneBinding = new EventBinding<CameraZoneChangedEvent>(OnZoneChanged);
            EventBus<CameraZoneChangedEvent>.Register(_zoneBinding);
        }

        private void OnDisable()
        {
            Player.ActivePlayerChanged -= OnPlayerActive;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventBus<CameraZoneChangedEvent>.Deregister(_zoneBinding);
        }

        private void Start()
        {
            StartCoroutine(InitializeAfterCinemachine());
        }

        private IEnumerator InitializeAfterCinemachine()
        {
            yield return null;

            var brain = Camera.main.GetComponent<CinemachineBrain>();
            if (brain == null) yield break;

            _virtualCamera = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (_virtualCamera == null) yield break;

            // Ensure confiner component exists on the virtual camera
            _confiner = _virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (_confiner == null)
                _confiner = _virtualCamera.gameObject.AddComponent<CinemachineConfiner2D>();
            _confiner.m_Damping = 0f;

            if (!_aspectApplied)
            {
                float screenAspect = (float)Screen.width / Screen.height;
                var lens = _virtualCamera.m_Lens;
                lens.OrthographicSize *= TargetAspect / screenAspect;
                _virtualCamera.m_Lens = lens;
                _aspectApplied = true;
            }

            if (_pendingFollowTarget != null)
            {
                _virtualCamera.Follow = _pendingFollowTarget;
                _pendingFollowTarget = null;
            }

            // Assign the bounding shape for the initial scene
            ApplyConfineBounds();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(ApplyConfineBoundsNextFrame());
        }

        // Wait one frame so scene objects finish their Awake/Start before we search for them.
        private IEnumerator ApplyConfineBoundsNextFrame()
        {
            yield return null;
            ApplyConfineBounds();
        }

        private void ApplyConfineBounds()
        {
            if (_confiner == null) return;

            var bounds = FindObjectOfType<CameraConfinerBounds>();
            if (bounds == null) return;

            _confiner.m_BoundingShape2D = bounds.BoundingShape;
            _confiner.InvalidateCache();

            // Force Cinemachine to immediately re-evaluate so the confiner
            // snaps the camera inside the new bounds before the first rendered frame.
            Camera.main?.GetComponent<CinemachineBrain>()?.ManualUpdate();
        }

        private void OnZoneChanged(CameraZoneChangedEvent e)
        {
            if (_confiner == null) return;
            _confiner.m_BoundingShape2D = e.BoundingShape;
            _confiner.InvalidateCache();
            Camera.main?.GetComponent<CinemachineBrain>()?.ManualUpdate();
        }

        private void OnPlayerActive(Player player)
        {
            if (_virtualCamera == null)
            {
                _pendingFollowTarget = player.transform;
                return;
            }

            _virtualCamera.Follow = player.transform;
        }
    }
}
