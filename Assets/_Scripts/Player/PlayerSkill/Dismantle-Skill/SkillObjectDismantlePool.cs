using Base;
using UnityEngine;

/// <summary>
/// Manages a single <see cref="ObjectPool{T}"/> for <see cref="SkillObjectDismantle"/> instances.
/// Attach alongside <see cref="SkillDismantle"/> and assign <see cref="_prefab"/> in the Inspector.
/// </summary>
public class SkillObjectDismantlePool : MonoBehaviour
{
    [SerializeField] private SkillObjectDismantle _prefab;

    private ObjectPool<SkillObjectDismantle> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<SkillObjectDismantle>(
            createFunc: Create,
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => { obj.gameObject.SetActive(false); },
            actionOnDestroy: obj => { if (obj != null) Destroy(obj.gameObject); }
        );
    }

    /// <summary>Returns an active <see cref="SkillObjectDismantle"/> instance from the pool.</summary>
    public SkillObjectDismantle Get()
    {
        var instance = _pool.Get();
        instance.Init(() => _pool.Release(instance));
        return instance;
    }

    private SkillObjectDismantle Create()
    {
        var instance = Instantiate(_prefab, transform);
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnDestroy()
    {
        _pool?.Dispose();
    }
}
