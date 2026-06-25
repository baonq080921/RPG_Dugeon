using System.Collections;
using Interfaces;
using UnityEngine;
using Base;
using Pool;
using Unity.VisualScripting;

/// <summary>
/// Base class for entity visual effects. Handles material swapping for hit flash.
/// </summary>
public  class EntityVfx : MonoBehaviour
{
    protected  Material knockBackMat;

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
    private Coroutine _statusEffectCoroutine;
    public bool isEffectDone {get; private set;}


    protected virtual void Awake()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        OriginalMaterial = SpriteRenderer.material;
        defaultHitColor = _hitColor;
    }

    public void CreateEffectVFX(GameObject effect, Transform target)
    {
        var instance = Instantiate(effect, target.position, Quaternion.identity);
        Destroy(instance, 2f);
    }

    public void CreateEffectLevelVFx()
    {
        ServiceLocator.Get<PoolManager>()?.levelupPool?.Spawn(transform);
    }

    public Color GetHitColor(ElementType elementType)
    {
        // Debug.Log(_electricHitColor);
        if(elementType == ElementType.Electric)
        {
            _hitColor = _electricHitColor;
            return _hitColor;
        }
        else
        {
            _hitColor = defaultHitColor;
            return _hitColor;
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

    public void SpawnHitEffect(Transform target, bool isCrit, Color color)
    {
        float randomX = Random.Range(-0.3f, 0.3f);
        float randomY = Random.Range(-0.7f, 0.7f);
        _hitColor = color;
        Vector2 randomHitOffSet = new Vector2(randomX, randomY);
        ServiceLocator.Get<PoolManager>()?.hitEffectPool?.SpawnHitEffect(target, _hitColor, randomHitOffSet, isCrit);
    }


}