using System.Collections;
using Base;
using UnityEngine;

namespace player
{
    /// <summary>
    /// Spawns pooled <see cref="AfterImageGhost"/> copies of the player sprite during the dash.
    /// Extends <see cref="MonoBehaviourPool{T}"/> to handle all pool lifecycle concerns.
    /// </summary>
    public class AfterImageEffect : MonoBehaviourPool<AfterImageGhost>
    {
        [SerializeField] private float _spawnInterval = 0.05f;
        [SerializeField] private float _ghostDuration = 0.3f;
        [SerializeField] private Color _ghostColor = new Color(0.3f, 0.7f, 1f, 0.7f);

        // Ghosts return to pool on their own via the tween callback — no need for double-release checks.
        protected override bool CollectionCheck => false;

        private SpriteRenderer _spriteRenderer;
        private Coroutine _spawnCoroutine;
        private WaitForSeconds _spawnWait;

        protected override void Awake()
        {
            base.Awake();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            // Cache once to avoid a WaitForSeconds allocation every spawn cycle.
            _spawnWait = new WaitForSeconds(_spawnInterval);
        }

        /// <inheritdoc/>
        protected override AfterImageGhost CreateInstance()
        {
            var ghost = new GameObject("AfterImageGhost").AddComponent<AfterImageGhost>();
            ghost.gameObject.SetActive(false);
            return ghost;
        }

        /// <inheritdoc/>
        protected override void OnRelease(AfterImageGhost ghost) => ghost.ResetState();

        /// <summary>Starts spawning ghost images. Call when the dash begins.</summary>
        public void StartEffect()
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = StartCoroutine(SpawnLoop());
        }

        /// <summary>Stops spawning ghost images. Call when the dash ends.</summary>
        public void StopEffect()
        {
            if (_spawnCoroutine == null) return;
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                SpawnGhost();
                yield return _spawnWait;
            }
        }

        private void SpawnGhost()
        {
            var ghost = Get();
            ghost.transform.SetPositionAndRotation(
                _spriteRenderer.transform.position,
                _spriteRenderer.transform.rotation
            );
            ghost.transform.localScale = _spriteRenderer.transform.lossyScale;

            ghost.Play(
                _spriteRenderer.sprite,
                _spriteRenderer.sortingLayerID,
                _spriteRenderer.sortingOrder - 1,
                _spriteRenderer.flipX,
                _ghostColor,
                _ghostDuration,
                g => Release(g)
            );
        }
    }
}
