using Base;
using enemy;
using Interfaces;
using player;
using UnityEngine;

/// <summary>
/// Base class for all spawned skill objects. Provides common references and a ground check.
/// </summary>
public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator anim;
  

    /// <summary>True when the object is resting on ground.</summary>
    /// 
    [Header("Attack Check")]
    
    [SerializeField] private float _attackRadius = 0.5f;
    [SerializeField] protected LayerMask _enemyLayer;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private bool _isDraw = true;
    protected virtual void Awake()
    {
        if (rb == null)
            rb = GetComponentInChildren<Rigidbody2D>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Update(){}


    public void DamageEnemiesInRadius(float damage,bool isCrit, bool appliesKnockback = true)
    {
        var hits = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRadius, _enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EnemyHealth>(out var health))
            {
                Debug.Log($"Damaging enemy {hit.name} for {damage} damage.");
                Transform target = appliesKnockback ? hit.transform : null;
                health?.TakeDamage(damage, 0, ElementType.None, target);
                EventBus<DamagePopupEvent>.Raise(new DamagePopupEvent(hit.transform.position,damage,isCrit));
            }
        }
    }


    public virtual void HandleDeath(){   }    
    protected virtual void OnDrawGizmos()
    {
        if(!_isDraw) return;
        if(_attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
    }

    public virtual void AttackTrigger(){}
    public virtual void AttackTrigger(bool canKnock){}
}
