using System.Collections.Generic;
using Base;
using enemy;
using UnityEngine;

namespace Pool
{
    /// <summary>
    /// Multi-type object pool for <see cref="Enemy"/> instances.
    /// Holds one <see cref="ObjectPool{T}"/> per <see cref="EnemyType"/>, each bound to its own prefab,
    /// so a single pool can spawn every enemy variant. Registers itself to <see cref="ServiceLocator"/>
    /// on <c>Awake</c> so spawners can retrieve it without a direct reference.
    /// </summary>
    public class EnemyPool : MonoBehaviour
    {
        /// <summary>Inspector mapping of an <see cref="EnemyType"/> to its prefab and per-type pool sizing.</summary>
        [System.Serializable]
        private struct EnemyEntry
        {
            [field: SerializeField] public EnemyType Type { get; private set; }
            [field: SerializeField] public Enemy Prefab { get; private set; }
            [field: SerializeField] public int DefaultCapacity { get; private set; }
            [field: SerializeField] public int MaxSize { get; private set; }
        }

        [SerializeField] private EnemyEntry[] _entries;

        // One pool per type so each enemy variant recycles only its own prefab instances.
        private readonly Dictionary<EnemyType, ObjectPool<Enemy>> _pools = new();

        private void Awake()
        {
            ServiceLocator.Register(this);
            BuildPools();
        }

        // Enemies are currently hand-placed in each map (no runtime spawner yet). Adopt them on Start
        // so that, on death, they recycle through the pool instead of reviving in place.
        private void Start()
        {
            foreach (var enemy in FindObjectsOfType<Enemy>())
                Adopt(enemy);
        }

        private void OnDestroy()
        {
            foreach (var pool in _pools.Values)
                pool.Dispose();
            _pools.Clear();
        }

        private void BuildPools()
        {
            foreach (var entry in _entries)
            {
                if (entry.Prefab == null)
                {
                    Debug.LogError($"[EnemyPool] Entry for {entry.Type} has no prefab assigned.");
                    continue;
                }
                if (_pools.ContainsKey(entry.Type))
                {
                    Debug.LogError($"[EnemyPool] Duplicate entry for enemy type {entry.Type}; extra prefab ignored.");
                    continue;
                }

                var prefab = entry.Prefab;
                _pools[entry.Type] = new ObjectPool<Enemy>(
                    createFunc: () => CreateInstance(prefab),
                    actionOnGet: enemy => { if (enemy != null) enemy.gameObject.SetActive(true); },
                    actionOnRelease: enemy => { if (enemy != null) enemy.gameObject.SetActive(false); },
                    actionOnDestroy: enemy => { if (enemy != null) Destroy(enemy.gameObject); },
                    defaultCapacity: Mathf.Max(1, entry.DefaultCapacity),
                    maxSize: Mathf.Max(1, entry.MaxSize));
            }
        }

        private Enemy CreateInstance(Enemy prefab)
        {
            var instance = Instantiate(prefab);
            instance.gameObject.SetActive(false);
            return instance;
        }

        /// <summary>
        /// Spawns an enemy of the requested <paramref name="type"/> at <paramref name="position"/>.
        /// </summary>
        /// <param name="type">Which enemy variant to take from the pool.</param>
        /// <param name="position">World-space spawn position.</param>
        /// <returns>The activated enemy, or <c>null</c> if no pool is configured for <paramref name="type"/>.</returns>
        public Enemy Spawn(EnemyType type, Vector3 position)
        {
            if (!_pools.TryGetValue(type, out var pool))
            {
                Debug.LogError($"[EnemyPool] No pool configured for enemy type {type}.");
                return null;
            }

            var enemy = pool.Get();
            enemy.transform.position = position;
            enemy.col.enabled = true;
            enemy.UnTargetableEnemy(false);
            // Route this instance back to its own sub-pool on death, keeping Enemy decoupled from the pool.
            enemy.OnDied = () => pool.Release(enemy);
            return enemy;
        }

        /// <summary>
        /// Brings a pre-placed scene enemy under pool management so it recycles through the pool on death.
        /// The enemy's <see cref="EnemyType"/> (from its <c>enemyData</c>) selects the matching sub-pool.
        /// Driven from the pool side so <see cref="Enemy"/> never references the pool.
        /// </summary>
        /// <param name="enemy">An already-active scene enemy to adopt.</param>
        public void Adopt(Enemy enemy)
        {
            if (enemy == null) return;
            if (enemy.enemyData == null)
            {
                Debug.LogWarning($"[EnemyPool] '{enemy.name}' has no EnemyData; not adopted.");
                return;
            }

            var type = enemy.enemyData.Type;
            if (!_pools.TryGetValue(type, out var pool))
            {
                Debug.LogWarning($"[EnemyPool] No pool configured for scene enemy type {type}; '{enemy.name}' not adopted.");
                return;
            }

            pool.Adopt(enemy);
            enemy.OnDied = () => pool.Release(enemy);
        }
    }
}
