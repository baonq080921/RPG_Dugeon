using System.Collections;
using Base;
using DG.Tweening;
using Interfaces;
using UnityEngine;

public class DomainExpasionSkillObject : MonoBehaviour
{
    [SerializeField] private float _expandDuration  = 1.5f;
    [SerializeField] private float _collapseDuration = 0.5f;
    private float _damage;
    private float _elementalDamage;
    [Range(0,20)]
    [SerializeField] private float _radiusDamge;
    [SerializeField] private LayerMask _targetLayer;
    private Vector3 _targetScale;
    private Coroutine _domainCoroutine;
    [SerializeField] private Collider2D[] hits;
    [SerializeField] private SpriteRenderer _shrineSr;
    private bool _isCrit;
    private void Awake()
    {
        _targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        Color tempColor = _shrineSr.color;
        tempColor.a = 0f;
        _shrineSr.color = tempColor;
    }

    public void OpenDomain(float duration)
    {
        if (_domainCoroutine != null)
            StopCoroutine(_domainCoroutine);
        _domainCoroutine = StartCoroutine(DomainRoutine(duration));
    }

    private IEnumerator DomainRoutine(float duration)
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;

        yield return transform.DOScale(_targetScale, _expandDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .WaitForCompletion();
        yield return _shrineSr.DOFade(1,0.25f)
        .SetEase(Ease.InQuad)
        .SetUpdate(true)
        .WaitForCompletion();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            DectectTargetInRadius();
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }

        yield return transform.DOScale(Vector3.zero, _collapseDuration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .WaitForCompletion();

        yield return _shrineSr.DOFade(0,0.25f)
        .SetEase(Ease.InQuad)
        .SetUpdate(true)
        .WaitForCompletion();
        _domainCoroutine = null;
        Destroy(gameObject);
    }

    public void SetUpDamageForDomain(float damage, float elementalDamage, float physicDamage, float scaleFactor,bool isCrit)
    {
        float secondScaleFactor = scaleFactor *0.3f;
        _isCrit = isCrit;
        if(physicDamage >= elementalDamage)
        {
            _damage = damage + physicDamage * scaleFactor;
            _elementalDamage = damage + elementalDamage *secondScaleFactor;
        }

        else
        {
            _damage = damage + physicDamage * secondScaleFactor;
            _elementalDamage = damage + elementalDamage *scaleFactor;            
        }
    }
    private void DectectTargetInRadius()
    {
        hits = Physics2D.OverlapCircleAll(transform.position,_radiusDamge,_targetLayer);
       for(int i = 0 ; i < hits.Length; i++)
        {
            var target = hits[i].GetComponent<IHit>();
            var targetTf = hits[i].GetComponent<Transform>();
            if(target != null)
            {
                target?.TakeDamage(_damage,_elementalDamage,ElementType.None,transform);
                ServiceLocator.Get<PoolManager>()?.sliceEffectPool.Spawn(targetTf);
                float finalDamage = _damage +_elementalDamage;
                EventBus<DamagePopupEvent>.Raise(new DamagePopupEvent(targetTf.position,finalDamage,_isCrit));
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,_radiusDamge);
    }
}
