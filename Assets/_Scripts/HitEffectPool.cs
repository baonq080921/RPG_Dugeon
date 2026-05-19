using System.Threading.Tasks;
using Base;
using UnityEngine;

/// <summary>
/// Manages two separate object pools for normal and critical <see cref="HitEffect"/> instances.
/// Registers itself to <see cref="ServiceLocator"/> on Awake.
/// </summary>
public class HitEffectPool : MonoBehaviourPool<HitEffect>
{
    [SerializeField] private HitEffect _hitPrefab;
    [SerializeField] private HitEffect _hitCritPrefab;

    private ObjectPool<HitEffect> _critPool;

    protected override void Awake()
    {
        base.Awake();
        _critPool = new ObjectPool<HitEffect>(
            createFunc: () => { var h = Instantiate(_hitCritPrefab); h.gameObject.SetActive(false); return h; },
            actionOnGet: h => { h.gameObject.SetActive(true); h.EnableHit(); },
            actionOnRelease: h => { h.DisableHit(); h.transform.SetParent(null); h.gameObject.SetActive(false); },
            actionOnDestroy: h => { if (h != null) Destroy(h.gameObject); });
        ServiceLocator.Register<HitEffectPool>(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _critPool?.Dispose();
    }

    /// <inheritdoc/>
    protected override HitEffect CreateInstance()
    {
        var hit = Instantiate(_hitPrefab);
        hit.gameObject.SetActive(false);
        return hit;
    }

    // Start the animation the moment the object is activated.
    protected override void OnGet(HitEffect hit) => hit.EnableHit();

    // Reset state and un-parent before the object goes back to the pool.
    protected override void OnRelease(HitEffect hit)
    {
        hit.DisableHit();
        hit.ResetColor();
        hit.transform.SetParent(null);
    }

    /// <summary>
    /// Spawns a normal or critical hit effect at <paramref name="targetTf"/> and returns it to the pool after 1 second.
    /// </summary>
    /// <param name="targetTf">The transform to attach the effect to.</param>
    /// <param name="hitColor">Color tint applied to the effect sprite.</param>
    /// <param name="randomHitOffSet">Local-space offset from the target's origin.</param>
    /// <param name="isCrit">When true, spawns from the critical hit pool.</param>
    public void SpawnHitEffect(Transform targetTf, Color hitColor, Vector2 randomHitOffSet, bool isCrit)
    {
        var hit = isCrit ? _critPool.Get() : Get();
        hit.transform.SetParent(targetTf);
        // hit.SetRandomRotation();
        if (!isCrit)
        {
            hit.SetRandomPositionVFX(randomHitOffSet, hit);
            hit.SetRandomRotation();
            hit.sr.color = hitColor;
        }else{
            hit.transform.localPosition = Vector3.zero;
        }
    
        ReturnAfterDelay(hit, isCrit);
    }

    private async void ReturnAfterDelay(HitEffect hit, bool isCrit)
    {
        await Task.Delay(1000);
        if (hit == null) return;
        if (isCrit)
            _critPool.Release(hit);
        else
            Release(hit);
    }
}
