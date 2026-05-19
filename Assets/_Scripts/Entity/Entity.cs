using Interfaces;
using UnityEngine;
using stateMachine;
using Base;
using System.Collections;
using System;

/// <summary>
/// Base class for all game entities (players and enemies).
/// Implements <see cref="IHit"/> so any entity can receive damage via the interface.
/// Subclasses must supply <see cref="MaxHealth"/> and <see cref="Damage"/> from their ScriptableObject data.
/// </summary>
public abstract class Entity : MonoBehaviour
{

    public StateMachine stateMachine {get; private set;}
    public Animator animator {get; private set;}

    public Rigidbody2D rb {get; private set;}
    public Collider2D col {get; private set;}
    [SerializeField] protected LayerMask _whatIsGround;
    [SerializeField] protected LayerMask _whatIsWall;
    [SerializeField] protected Transform _groundCheckPoint;

    [Space]
    [Range(0,1f)]
    [SerializeField] protected float _groundCheckRadius = 0.1f;

    [Space]
    [Range(0,10f)]
    [SerializeField] protected float _wallCheckDistance = 0.5f;


    public float direction {get; protected set;} = 1f; // 1 for right, -1 for left
    public void SetDirection(float dir) => direction = dir;

    public bool isGrounded{get; protected set;}
    public bool isTouchingWall {get; protected set;}
    private Vector3 _originalScale;
    public abstract LayerMask LayerMask { get; }
    /// <summary>Whether attacks from this entity apply knockback to the target.</summary>
    [field:SerializeField]public bool CanKnockBackOnHit { get; protected set; } = true;

    public event Action OnFlip;
    private Coroutine _knockBackCoroutine;
    public bool IsKnocked { get; private set; }
    [SerializeField] private Vector2 _knockBackPowerLight;
    [SerializeField] private Vector2 _knockBackPowerHeavy;


    public EntityStat entityStat { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        _originalScale = transform.localScale;
        col = GetComponent<Collider2D>();
        entityStat = GetComponent<EntityStat>();

    }

    protected virtual void Start()
    {
    }



    protected virtual void Update()
    {
        stateMachine.currentState.Update();
        CheckGrounded();
        CheckWall();
    }

    public void Flip(float direction)
    {
        transform.localScale = new Vector3(_originalScale.x * direction, _originalScale.y, _originalScale.z) ;
        OnFlip?.Invoke();
    }

    public virtual void SetVelocity(Vector2 velocity)
    {
        if(IsKnocked) return;
        
        rb.velocity = velocity;
    }


    public void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _whatIsGround);
    }
    public void CheckWall()
    {
        isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * direction, _wallCheckDistance, _whatIsWall);
    }

    public void TriggerAnimationEvent()
    {
        stateMachine.currentState.TriggerAnimation();
    }

    public virtual void Die(){}



    public virtual void ApplyKnockBack(float damage)
        {
            float ratio = damage / entityStat.GetHealthValue();
            // Heavy when hit is a  bigger fraction of max health, light otherwise
            Vector2 power = ratio < entityStat.GetKnockBackThreshHold()
                ?  _knockBackPowerLight
                :_knockBackPowerHeavy;
            // Negate x so the enemy is pushed away from the player (enemy faces toward player)
            Vector2 knockBack = new Vector2(power.x * - direction, power.y);
            ReciveKnockBack(knockBack,entityStat.StunDuration);
        }



    private void ReciveKnockBack(Vector2 knockBack, float duration)
    {
        if(_knockBackCoroutine != null)
            StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(KnockBackCo(knockBack, duration));
    }

    private IEnumerator KnockBackCo(Vector2 knockBack, float duration)
    {
        IsKnocked = true;
        rb.velocity =  knockBack;
        yield return new WaitForSeconds(duration);
        IsKnocked = false;
        rb.velocity =  Vector2.zero;
    }

    /// <summary>Cancels any active knockback coroutine and clears the knocked state.</summary>
    protected void ResetKnockbackState()
    {
        if (_knockBackCoroutine != null)
            StopCoroutine(_knockBackCoroutine);
        IsKnocked = false;
    }

    /// <summary>Sets all Animator bool parameters to false for a clean state on pool re-use.</summary>
    protected void ResetAllAnimatorBools()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
                animator.SetBool(param.name, false);
        }
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position,_wallCheckDistance * Vector2.right * direction + (Vector2)transform.position);


        if(isGrounded){
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);           
    }    
}