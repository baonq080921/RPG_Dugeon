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

    [Header("Elemental Hit VFX")]
    [SerializeField] private Color _electricHitColor;
    [SerializeField] private float _electricBlinkInterval = 0.15f;
    private Color defaultHitColor;
    private EntityCombat _entityCombat;
    private Coroutine _electricStatusEffectCoroutine;


    protected virtual void Awake()
    {
        ServiceLocator.Register(this);
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        OriginalMaterial = SpriteRenderer.material;
        defaultHitColor = _hitColor;
        _entityCombat = GetComponent<EntityCombat>();
    }

    protected virtual void OnEnable()
    {
        _entityCombat.OnTargetHit += CreateHitEffect;
    }

    protected virtual void OnDisable()
    {
        _entityCombat.OnTargetHit -= CreateHitEffect;
    }

    public void UpdateHitColor(ElementType elementType)
    {
        // Debug.Log(_electricHitColor);
        if(elementType == ElementType.Electric)
        {
            _hitColor = _electricHitColor;
        }
        else
        {
            _hitColor = defaultHitColor;
        }
    }

    //Status effect vfx can be implemented here as well, for example we can change the color of the entity when it is under electric status effect, and change it back when the status effect ends. We can also spawn some particle effects to indicate the status effect.
    public void UpdateStatusEffectVFX(ElementType elementType,float duration)
    {
        if(elementType == ElementType.Electric)
        {
            PlayElectricCoroutine(duration);
        }
    }

    private void PlayElectricCoroutine(float duration)
    {
        if (_electricStatusEffectCoroutine != null)
            StopCoroutine(_electricStatusEffectCoroutine);
        _electricStatusEffectCoroutine = StartCoroutine(ElectricStatusEffectVFX(duration));
    }

    private IEnumerator ElectricStatusEffectVFX(float duration)
    {
        float elapsed = 0f;
        bool useBright = true;
        Color darkColor = new Color(_electricHitColor.r * 0.5f, _electricHitColor.g * 0.5f, _electricHitColor.b * 0.5f, _electricHitColor.a);

        while (elapsed < duration)
        {
            SpriteRenderer.color = useBright ? _electricHitColor : darkColor;
            useBright = !useBright;
            yield return new WaitForSeconds(_electricBlinkInterval);
            elapsed += _electricBlinkInterval;
        }

        SpriteRenderer.color = Color.white;
        _electricStatusEffectCoroutine = null;
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