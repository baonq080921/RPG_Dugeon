using System.Collections;
using Cinemachine;
using player;
using UnityEngine;

namespace Base
{
    /// <summary>
    /// Adjusts the Cinemachine Virtual Camera orthographic size once at startup so the
    /// horizontal world extent is always consistent with a 16:9 landscape reference,
    /// regardless of the device's actual screen ratio.
    /// </summary>
    public class CameraResolutionFitter : MonoBehaviour
    {
        private const float TargetAspect = 16f / 9f;

        private CinemachineVirtualCamera _virtualCamera;
        private Transform _pendingFollowTarget;

        private void OnEnable()
        {
            Player.ActivePlayerChanged += OnPlayerActive;
        }

        private void OnDisable()
        {
            Player.ActivePlayerChanged -= OnPlayerActive;
        }

        private void Start()
        {
            StartCoroutine(InitializeAfterCinemachine());
        }

        private IEnumerator InitializeAfterCinemachine()
        {
            yield return null;

            var brain = Camera.main.GetComponent<CinemachineBrain>();

            if (brain == null)
                yield break;

            _virtualCamera = brain.ActiveVirtualCamera as CinemachineVirtualCamera;

            if (_virtualCamera == null)
                yield break;

            float screenAspect = (float)Screen.width / Screen.height;
            var lens = _virtualCamera.m_Lens;
            lens.OrthographicSize *= TargetAspect / screenAspect;
            _virtualCamera.m_Lens = lens;

            if (_pendingFollowTarget != null)
            {
                _virtualCamera.Follow = _pendingFollowTarget;
                _pendingFollowTarget = null;
            }
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
