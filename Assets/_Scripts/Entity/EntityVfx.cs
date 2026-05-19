using System.Collections;
using Interfaces;
using UnityEngine;
using Base;
using Unity.VisualScripting;

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
    [SerializeField] private Color _hitColor;


    protected virtual void Awake()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        OriginalMaterial = SpriteRenderer.material;
    }

    protected virtual void OnEnable()
    {
        ServiceLocator.Get<EntityCombat>().OnTargetHit += CreateHitEffect;
    }

    protected virtual void OnDisable()
    {
        ServiceLocator.Get<EntityCombat>().OnTargetHit -= CreateHitEffect;
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



    private void CreateHitEffect(Transform target, bool isCrit)
    {
        SpawnHitEffect(target, isCrit);
    }

    private void SpawnHitEffect(Transform target, bool isCrit)
    {
        float randomX = Random.Range(-0.3f, 0.3f);
        float randomY = Random.Range(-0.7f, 0.7f);
        Vector2 randomHitOffSet = new Vector2(randomX, randomY);
        ServiceLocator.Get<HitEffectPool>().SpawnHitEffect(target, _hitColor, randomHitOffSet, isCrit);
    }


}