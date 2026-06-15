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
    [SerializeField]private Color _iceHitColor;
    [SerializeField]private Color _fireColor;
    [SerializeField] private float _statusBlinkInterval = 0.15f;
    private Color defaultHitColor;
    private EntityCombat _entityCombat;
    private Entity _entity;
    private Coroutine _statusEffectCoroutine;
    private EventBinding<TargetGotHitEvent> _hitEventBinding;
    public bool isEffectDone {get; private set;}


    protected virtual void Awake()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        OriginalMaterial = SpriteRenderer.material;
        defaultHitColor = _hitColor;
        _entityCombat = GetComponent<EntityCombat>();
        _entity = GetComponent<Entity>();
    }

    protected virtual void OnEnable()
    {
        _hitEventBinding = new EventBinding<TargetGotHitEvent>(OnHitCreate);
        EventBus<TargetGotHitEvent>.Register(_hitEventBinding);

        // _entityCombat.OnTargetHit += CreateHitEffect;
    }

    protected virtual void OnDisable()
    {
        EventBus<TargetGotHitEvent>.Deregister(_hitEventBinding);
        // _entityCombat.OnTargetHit -= CreateHitEffect;
    }

    public void CreateEffectVFX(GameObject effect, Transform target)
    {
        var instance = Instantiate(effect, target.position, Quaternion.identity);
        Destroy(instance, 2f);
    }

    public void CreateEffectLevelVFx()
    {
        ServiceLocator.Get<PoolManager>()?.levelupPool?.Spawn(transform.position);
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
        Color statusColor;
        if(elementType == ElementType.Electric)
        {
            statusColor = _electricHitColor;
            PlayEffectStatusCoroutine(duration,statusColor);
        }
        if(elementType == ElementType.Ice)
        {
            statusColor =_iceHitColor;
            PlayEffectStatusCoroutine(duration,statusColor);
        }
        if(elementType == ElementType.Fire)
        {
            statusColor = _fireColor;
            PlayEffectStatusCoroutine(duration,statusColor);
        }
    }

    private void PlayEffectStatusCoroutine(float duration, Color statusColor)
    {
        if (_statusEffectCoroutine != null)
            StopCoroutine(_statusEffectCoroutine);
        _statusEffectCoroutine = StartCoroutine(StatusEffectVFX(duration,statusColor));
    }

    private IEnumerator StatusEffectVFX(float duration,Color statusColor)
    {
        float elapsed = 0f;
        bool useBright = true;
        Color darkColor = new Color(statusColor.r * 0.5f, _electricHitColor.g * 0.5f, _electricHitColor.b * 0.5f, _electricHitColor.a);
        isEffectDone = false;
        while (elapsed < duration)
        {
            SpriteRenderer.color = useBright ? statusColor : darkColor;
            useBright = !useBright;
            yield return new WaitForSeconds(_statusBlinkInterval);
            elapsed += _statusBlinkInterval;
        }
        SpriteRenderer.color = Color.white;
        isEffectDone = true;
        _statusEffectCoroutine = null;
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

    private void OnHitCreate(TargetGotHitEvent hit)
    {
        CreateHitEffect(hit.target,hit.isCrit);
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
        ServiceLocator.Get<PoolManager>()?.hitEffectPool?.SpawnHitEffect(target, _hitColor, randomHitOffSet, isCrit);
    }


}