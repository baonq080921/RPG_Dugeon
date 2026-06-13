using Base;
using UnityEngine;

/// <summary>
/// Pool for <see cref="LevelUpEffect"/> VFX instances.
/// Assigned to <see cref="PoolManager"/> in the Inspector — do not register with ServiceLocator directly.
/// </summary>
public class LevelUpEffectPool : MonoBehaviourPool<LevelUpEffect>
{
    [SerializeField] private LevelUpEffect _prefab;

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
    public void Spawn(Vector3 position)
    {
        var item = Get();
        item.transform.position = position;
    }
}
