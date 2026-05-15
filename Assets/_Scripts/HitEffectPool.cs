using System.Threading.Tasks;
using Base;
using UnityEngine;

/// <summary>
/// Object pool for <see cref="HitEffect"/> instances.
/// Registers itself to <see cref="ServiceLocator"/> on Awake.
/// </summary>
public class HitEffectPool : MonoBehaviourPool<HitEffect>
{
    [SerializeField] private HitEffect _hitPrefab;

    protected override void Awake()
    {
        base.Awake();
        ServiceLocator.Register<HitEffectPool>(this);
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
        hit.transform.SetParent(null);
    }

    /// <summary>
    /// Spawns a hit effect at <paramref name="targetTf"/> and automatically returns it to the pool after 1 second.
    /// </summary>
    /// <param name="targetTf">The transform to attach the effect to.</param>
    public void SpawnHitEffect(Transform targetTf,Color hitColor, Vector2 randomHitOffSet)
    {
        var hit = Get();
        hit.transform.SetParent(targetTf);
        hit.transform.localPosition = Vector3.zero + (Vector3)randomHitOffSet;
        hit.sr.color = hitColor;
        ReturnAfterDelay(hit);
    }

    private async void ReturnAfterDelay(HitEffect hit)
    {
        await Task.Delay(1000);
        if (hit != null)
            Release(hit);
    }
}
