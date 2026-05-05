using System.Collections;
using Base;
using UnityEngine;

namespace player
{
    /// <summary>
    /// Spawns pooled <see cref="AfterImageGhost"/> copies of the player sprite during the dash.
    /// Uses <see cref="ObjectPool{T}"/> to avoid per-ghost Instantiate/Destroy GC pressure.
    /// </summary>
    public class AfterImageEffect : MonoBehaviour
    {
        [SerializeField] private float _spawnInterval = 0.05f;
        [SerializeField] private float _ghostDuration = 0.3f;
        [SerializeField] private Color _ghostColor = new Color(0.3f, 0.7f, 1f, 0.7f);
        [SerializeField] private int _poolDefaultCapacity = 10;
        [SerializeField] private int _poolMaxSize = 20;

        private SpriteRenderer _spriteRenderer;
        private ObjectPool<AfterImageGhost> _pool;
        private Coroutine _spawnCoroutine;
        private WaitForSeconds _spawnWait;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            // Cache once to avoid a WaitForSeconds allocation every spawn cycle.
            _spawnWait = new WaitForSeconds(_spawnInterval);

            _pool = new ObjectPool<AfterImageGhost>(
                createFunc: CreateGhost,
                actionOnGet: ghost => ghost.gameObject.SetActive(true),
                actionOnRelease: ghost =>
                {
                    ghost.ResetState();
                    ghost.gameObject.SetActive(false);
                },
                actionOnDestroy: ghost => Destroy(ghost.gameObject),
                collectionCheck: false,
                defaultCapacity: _poolDefaultCapacity,
                maxSize: _poolMaxSize
            );
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        /// <summary>Starts spawning ghost images. Call when dash begins.</summary>
        public void StartEffect()
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = StartCoroutine(SpawnLoop());
        }

        /// <summary>Stops spawning ghost images. Call when dash ends.</summary>
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
            var ghost = _pool.Get();
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
                ReturnToPool
            );
        }

        private AfterImageGhost CreateGhost()
        {
            var ghost = new GameObject("AfterImageGhost").AddComponent<AfterImageGhost>();
            ghost.gameObject.SetActive(false);
            return ghost;
        }

        private void ReturnToPool(AfterImageGhost ghost)
        {
            _pool.Release(ghost);
        }
    }
}
