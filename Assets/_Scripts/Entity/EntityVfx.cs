using System.Collections;
using Interfaces;
using UnityEngine;

/// <summary>
/// Base class for entity visual effects. Handles material swapping for hit flash.
/// </summary>
public abstract class EntityVfx : MonoBehaviour,IHitVFX
{
    protected abstract Material KnockBackMat { get; }

    [SerializeField] protected float HitFlashDuration = 0.3f;

    protected SpriteRenderer SpriteRenderer { get; private set; }
    protected Material OriginalMaterial { get; private set; }

     private Coroutine _hitVfxCoroutine;

    protected virtual void Awake()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        OriginalMaterial = SpriteRenderer.material;
    }

     /// <inheritdoc/>
        public virtual void PlayHitVFX()
        {
            if (_hitVfxCoroutine != null)
                StopCoroutine(_hitVfxCoroutine);
            _hitVfxCoroutine = StartCoroutine(HitVfxCoroutine());
        }

        private IEnumerator HitVfxCoroutine()
        {
            SpriteRenderer.material = KnockBackMat;
            yield return new WaitForSeconds(HitFlashDuration);
            SpriteRenderer.material = OriginalMaterial;
            _hitVfxCoroutine = null;

        }
}