using Base;
using UnityEngine;

/// <summary>
/// Pool for <see cref="SliceEffect"/> VFX instances.
/// Assigned to <see cref="PoolManager"/> in the Inspector — do not register with ServiceLocator directly.
/// </summary>
public class SliceEffectPool : MonoBehaviourPool<SliceEffect>
{
    [SerializeField] private SliceEffect _prefab;

    protected override void Awake()
    {
        base.Awake();
        var manager = ServiceLocator.Get<PoolManager>();
        if (manager != null) manager.sliceEffectPool = this;
    }

    private void OnDestroy()
    {
        var manager = ServiceLocator.Get<PoolManager>();
        if (manager != null && manager.sliceEffectPool == this)
            manager.sliceEffectPool = null;
    }

    protected override SliceEffect CreateInstance()
    {
        var item = Instantiate(_prefab, transform);
        item.gameObject.SetActive(false);
        return item;
    }

    protected override void OnGet(SliceEffect item)
    {
        item.ResetAnimation();
        item.OnComplete = () => Release(item);
    }

    protected override void OnRelease(SliceEffect item)
    {
        item.OnComplete = null;
        item.transform.SetParent(null);
    }

    /// <summary>Spawns a level-up effect at <paramref name="position"/>.</summary>
    public void Spawn(Transform targetTf)
    {
        var item = Get();
        item.transform.position = targetTf.position;
        item.transform.SetParent(targetTf);
    }
}
