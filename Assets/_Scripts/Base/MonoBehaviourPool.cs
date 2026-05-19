using UnityEngine;

namespace Base
{
  /// <summary>
  /// Abstract generic pool for Unity <see cref="MonoBehaviour"/> components.
  /// Handles <see cref="Object.Destroy"/> and <see cref="GameObject.SetActive"/> so
  /// concrete subclasses only need to implement <see cref="CreateInstance"/>, and
  /// optionally <see cref="OnGet"/> / <see cref="OnRelease"/> for state reset or cleanup.
  /// </summary>
  /// <typeparam name="T">The <see cref="MonoBehaviour"/> type to pool.</typeparam>
  public abstract class MonoBehaviourPool<T> : MonoBehaviour where T : MonoBehaviour
  {
    [SerializeField] private int _defaultCapacity = 5;
    [SerializeField] private int _maxSize = 20;

    // Override in subclasses that don't need double-release protection (e.g. high-frequency VFX pools).
    protected virtual bool CollectionCheck => true;

    private ObjectPool<T> _pool;

    protected virtual void Awake()
    {
      _pool = new ObjectPool<T>(
        createFunc: CreateInstance,
        actionOnGet: item => { item.gameObject.SetActive(true); OnGet(item); },
        actionOnRelease: item => { OnRelease(item); item.gameObject.SetActive(false); },
        actionOnDestroy: item => { if (item != null) Destroy(item.gameObject); },
        collectionCheck: CollectionCheck,
        defaultCapacity: _defaultCapacity,
        maxSize: _maxSize);
    }

    protected virtual void OnDestroy()
    {
      _pool?.Dispose();
    }

    /// <summary>Creates a brand-new instance when the pool has no idle objects available.</summary>
    protected abstract T CreateInstance();

    /// <summary>Retrieves an instance from the pool, creating one if none are available.</summary>
    protected T Get() => _pool.Get();

    /// <summary>Returns an instance to the pool.</summary>
    protected void Release(T item) => _pool.Release(item);

    /// <summary>Called immediately after an instance is activated. Override to reset object state.</summary>
    protected virtual void OnGet(T item) { }

    /// <summary>Called immediately before an instance is deactivated. Override for pre-release cleanup.</summary>
    protected virtual void OnRelease(T item) { }
  }
}
