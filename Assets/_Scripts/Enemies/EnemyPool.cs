using System.Collections.Generic;
using Base;
using UnityEngine;

namespace enemy
{
  /// <summary>
  /// Object pool for <see cref="Enemy"/> instances.
  /// Registers itself to <see cref="ServiceLocator"/> on <c>Awake</c> so enemies can
  /// retrieve it without a direct reference.
  /// </summary>
  public class EnemyPool : MonoBehaviourPool<Enemy>
  {
    [SerializeField] private Enemy _prefab;

    private readonly HashSet<Enemy> _activeEnemies = new();

    protected override void Awake()
    {
      base.Awake();
      ServiceLocator.Register(this);
    }

    /// <inheritdoc/>
    protected override Enemy CreateInstance()
    {
      var instance = Instantiate(_prefab);
      instance.gameObject.SetActive(false);
      return instance;
    }

    /// <inheritdoc/>
    protected override void OnGet(Enemy enemy) => enemy.OnGetFromPool();

    /// <summary>
    /// Retrieves an enemy from the pool and places it at <paramref name="position"/>.
    /// </summary>
    /// <param name="position">World-space spawn position.</param>
    /// <returns>The activated, fully reset enemy instance.</returns>
    public Enemy Spawn(Vector3 position)
    {
      var enemy = Get();
      enemy.transform.position = position;
      _activeEnemies.Add(enemy);
      return enemy;
    }

    /// <summary>
    /// Returns an enemy to the pool. Destroys it instead if it was not spawned through this pool.
    /// </summary>
    /// <param name="enemy">The enemy to return.</param>
    public void Return(Enemy enemy)
    {
      if (!_activeEnemies.Remove(enemy))
      {
        Destroy(enemy.gameObject);
        return;
      }

      Release(enemy);
    }
  }
}
