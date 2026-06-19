using Base;
using UnityEngine;

/// <summary>
/// Pool for <see cref="LevelUpEffect"/> VFX instances.
/// Assigned to <see cref="PoolManager"/> in the Inspector — do not register with ServiceLocator directly.
/// </summary>
public class LevelUpEffectPool : MonoBehaviourPool<LevelUpEffect>
{
    [SerializeField] private LevelUpEffect _prefab;

    protected override void Awake()
    {
        base.Awake();
        var manager = ServiceLocator.Get<PoolManager>();
        if (manager != null) manager.levelupPool = this;
    }

    private void OnDestroy()
    {
        var manager = ServiceLocator.Get<PoolManager>();
        if (manager != null && manager.levelupPool == this)
            manager.levelupPool = null;
    }

    protected override LevelUpEffect CreateInstance()
    {
        var item = Instantiate(_prefab, transform);
        item.gameObject.SetActive(false);
        return item;
    }

    protected override void OnGet(LevelUpEffect item)
    {
        item.ResetAnimation();
        item.OnComplete = () => Release(item);
    }

    protected override void OnRelease(LevelUpEffect item)
    {
        item.OnComplete = null;
    }

    /// <summary>Spawns a level-up effect at <paramref name="position"/>.</summary>
    public void Spawn(Transform tf)
    {
        var item = Get();
        item.transform.position = tf.position;
        item.transform.SetParent(tf);
    }
}
