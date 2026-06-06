using System;
using System.Collections;
using player;
using UnityEngine;

public class SkillObjectTimeEcho : SkillObject_Base
{
    [SerializeField] private SkillBaseDefinition _skillTimeEchoDefinition;
    [Range(0,10f)]
    [SerializeField] private float _dectectionRange = 5f;
    [SerializeField] private float _stoppingDistance = 0.5f;
    [SerializeField] private float _moveSpeed = 5f;
    private Coroutine _deathCoroutine;
    private Transform _currentTarget;
    private Action _onRelease;

    /// <summary>Called by the pool after getting this instance. Wires up the release callback.</summary>
    public void Init(Action onRelease) => _onRelease = onRelease;

    /// <summary>Resets all runtime state so this instance is safe for pool reuse.</summary>
    public void ResetObject()
    {
        _currentTarget = null;
        if (_deathCoroutine != null)
        {
            StopCoroutine(_deathCoroutine);
            _deathCoroutine = null;
        }
        if (rb != null) rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("CanRun", false);
            anim.ResetTrigger("Attack");
        }
    }

    /// <inheritdoc/>
    public override void HandleDeath()
    {
        SpawnDeathVfx();
        ResetObject();
        _onRelease?.Invoke();
    }


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        if (anim != null)
        {
            if(IsGrounded) anim.SetFloat("yVelocity", 1);
            else anim.SetFloat("yVelocity", rb.velocity.y);
        }

        if (_currentTarget != null)
            PursueTarget();
    }

    public void SpawnTimeEcho(Transform spawnTransform = null)
    {
        // Implement the logic to spawn the time echo effect here.
        // This could involve instantiating a prefab, playing an animation, etc.
        transform.position = spawnTransform != null ? spawnTransform.position : Vector3.zero;
    }

    public void TimeEchoBase(Transform spawnTransform = null)
    {
        SpawnTimeEcho(spawnTransform);
        // Additional logic for the time echo effect can be added here.
        // For example, you might want to set a timer to destroy this object after a certain duration.
        
    }

    public void TimeEchoSideKickAttack(Transform spawnTransform = null, float damage = 0f)
    {
        // Implement the logic for the extra echo attack here.
        // This could involve creating a new attack hitbox, applying damage to enemies, etc.
        SpawnTimeEcho(spawnTransform);
        //Dectect enemies in range:
        DetectEnemiesInRange();
    }


    private void DetectEnemiesInRange()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _dectectionRange, _enemyLayer);
        if (hits.Length == 0) 
        {
             DeathEffect();
            return;
        }

        _currentTarget = GetNearestTarget(hits);
        Debug.Log($"Enemy Detected for Time Echo Side Kick Attack! {_currentTarget.name} ");
    }

    private void DeathEffect()
    {
        // Implement the logic for the death effect here.
        // This could involve playing a death animation, spawning particles, etc.
        if(_deathCoroutine != null) return;
        _deathCoroutine = StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        // Play death animation or effect here
        // Wait for the animation to finish (assuming 1 second here, adjust as needed)
        yield return new WaitForSeconds(1f);
        HandleDeath();
    }

    private Transform GetNearestTarget(Collider2D[] hits)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist >= minDist) continue;
            minDist = dist;
            nearest = hit.transform;
        }
        return nearest;
    }

    private void PursueTarget()
    {
        if (_currentTarget == null) return;

        float dist = Vector2.Distance(transform.position, _currentTarget.position);
        if (dist <= _stoppingDistance)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            anim.SetBool("CanRun", false);
            anim.SetTrigger("Attack");
            _currentTarget = null;
            return;
        }

        FlipToFace(_currentTarget);
        anim.SetBool("CanRun", true);
        float dirX = _currentTarget.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(dirX * _moveSpeed, rb.velocity.y);
    }

    private void FlipToFace(Transform target)
    {
        float dirX = target.position.x - transform.position.x;
        Vector3 scale = transform.localScale;
        scale.x = dirX > 0f ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }


    public void TimeEchoHealing(Transform spawnTransform = null)
    {
        // Implement the logic for the extra healing echo here.
        // This could involve creating a healing area, applying healing to the player, etc.
        Debug.Log("Extra Healing Echo Spawned!");
    }
    public void TimeEChoHealingCoolDown(Transform spawnTransform = null)
    {
        // Implement the logic for the extra echo effect here.
        // This could involve combining the attack and healing effects, or adding additional visual/audio effects.
        SpawnTimeEcho(spawnTransform);
        Debug.Log("Time Echo Healing Cooldown Started!");
    }

    public override void AttackTrigger()
    {
        base.AttackTrigger();
        float damge = _skillTimeEchoDefinition.Damage;
        DamageEnemiesInRadius(damge);
        Debug.Log("Time Echo Attack Triggered!");
    }



    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _dectectionRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * _stoppingDistance);
    }
    
    
    
}