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
    [SerializeField] private PoolableVfx _onDeathVfxPrefab;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private LayerMask _whatIsGround;
    [Range(0, 1f)]
    [SerializeField] private float _groundCheckRadius = 0.1f;
    /// <summary>True when the object is resting on ground.</summary>
    /// 
    [Header("Attack Check")]
    
    [SerializeField] private float _attackRadius = 0.5f;
    [SerializeField] protected LayerMask _enemyLayer;
    [SerializeField] private Transform _attackPoint;
    public bool IsGrounded { get; private set; }
    [SerializeField] private bool _isDraw = true;
    protected virtual void Awake()
    {
        if (rb == null)
            rb = GetComponentInChildren<Rigidbody2D>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        if(_groundCheckPoint != null)
            IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _whatIsGround);
    }


    public void DamageEnemiesInRadius(float damage, float radius  = 0.5f)
    {
        var hits = Physics2D.OverlapCircleAll(_attackPoint.position, radius, _enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EnemyHealth>(out var health))
            {
                Debug.Log($"Damaging enemy {hit.name} for {damage} damage.");
                health?.TakeDamage(damage,0, ElementType.None, transform);
            }
        }
    }


    public virtual void HandleDeath()
    {
        SpawnDeathVfx();
        Destroy(gameObject);
    }

    /// <summary>Spawns the death VFX via pool. Call from overrides before custom cleanup.</summary>
    protected void SpawnDeathVfx()
    {
        if (_onDeathVfxPrefab != null)
            PoolableVfx.Spawn(_onDeathVfxPrefab, transform.position);
    }

    protected virtual void OnDrawGizmos()
    {
        if(!_isDraw) return;
        if (_groundCheckPoint == null) return;
        if(_attackPoint == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
    }

    public virtual void AttackTrigger(){}
}
