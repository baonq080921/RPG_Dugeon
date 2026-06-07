using System;
using System.Collections;
using Base;
using player;
using UnityEngine;

public class SkillObjectTimeEcho : SkillObject_Base
{

    
    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private LayerMask _whatIsGround;
    [Range(0, 1f)]
    [SerializeField] private float _groundCheckRadius = 0.1f;

    public bool IsGrounded { get; private set; }


    [SerializeField] private SkillBaseDefinition _skillTimeEchoDefinition;
    [SerializeField] private GameObject _healingVfxPrefab;
    [Range(0,10f)]
    [SerializeField] private float _dectectionRange = 5f;
    [SerializeField] private float _stoppingDistance = 0.5f;
    [SerializeField] private float _moveSpeed = 5f;
    private Coroutine _deathCoroutine;
    private Transform _currentTarget;
    private bool _isMultiAttack;
    private float _multiAttackEndTime;

    [SerializeField] private PoolableVfx _onDeathVfxPrefab;
    private Action _onRelease;

    /// <summary>Called by the pool after getting this instance. Wires up the release callback.</summary>
    public void Init(Action onRelease) => _onRelease = onRelease;

    /// <summary>Resets all runtime state so this instance is safe for pool reuse.</summary>
    public void ResetObject()
    {
        _currentTarget = null;
        _isMultiAttack = false;
        _multiAttackEndTime = 0f;
        if (_deathCoroutine != null)
        {
            StopCoroutine(_deathCoroutine);
            _deathCoroutine = null;
        }
        if (rb != null) rb.velocity = Vector2.zero;
        if (anim != null)
        {
            // Rebind resets the Animator fully to its entry state, discarding any in-progress
            // animation or stale trigger/parameter values left over from the previous use.
            anim.Rebind();
            anim.Update(0f);
        }
    }

    /// <inheritdoc/>
    public override void HandleDeath()
    {
        base.HandleDeath();
        SpawnSmokeEffect();
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

        if(_groundCheckPoint != null)
            IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _whatIsGround);

        if (_currentTarget != null)
            PursueTarget();

        // Multi-attack: if duration expired with no target left, clean up
        if (_isMultiAttack && _currentTarget == null && Time.time > _multiAttackEndTime)
            EchoDisappearEffect();
    }

    public void SpawnTimeEcho(Transform spawnTransform = null, float offSetX = 0f)
    {
        // Implement the logic to spawn the time echo effect here.
        // This could involve instantiating a prefab, playing an animation, etc.
        transform.position = spawnTransform != null ? spawnTransform.position : Vector3.zero;
        transform.position += new Vector3(UnityEngine.Random.Range(-offSetX, offSetX), 0f, transform.position.z); // Apply horizontal offset if provided
    }

    public void TimeEchoBase(Transform spawnTransform = null)
    {
        SpawnTimeEcho(spawnTransform, UnityEngine.Random.Range(0.5f, 1.5f)); // Example of random horizontal offset for visual variety
        // Additional logic for the time echo effect can be added here.
        // For example, you might want to set a timer to destroy this object after a certain duration.    
        EchoDisappearEffect();    
    }



    #region Side Kick Attack Skill logic

    public void TimeEchoSideKickAttack(Transform spawnTransform = null, float damage = 0f)
    {
        // Implement the logic for the extra echo attack here.
        // This could involve creating a new attack hitbox, applying damage to enemies, etc.
        SpawnTimeEcho(spawnTransform);
        //Dectect enemies in range:
        DetectEnemiesInRange();
    }


    public void TimeEchoSideKickMahoragaAttack(Transform spawnTransform = null, float damage = 0f)
    {
        SpawnTimeEcho(spawnTransform);
        _isMultiAttack = true;
        _multiAttackEndTime = Time.time + _skillTimeEchoDefinition.Duration;
        DetectEnemiesInRange();
    }


    private void DetectEnemiesInRange()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _dectectionRange, _enemyLayer);
        if (hits.Length == 0) 
        {
             EchoDisappearEffect();
            return;
        }

        _currentTarget = GetNearestTarget(hits);
        Debug.Log($"Enemy Detected for Time Echo Side Kick Attack! {_currentTarget.name} ");
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
        if(!IsGrounded) return;
        float dist = Vector2.Distance(transform.position, _currentTarget.position);
        if (dist <= _stoppingDistance)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            anim.SetBool("CanRun", false);
            anim.SetTrigger("Attack");
            _currentTarget = null;
            // For multi-attack, skip the fallback — duration expiry in Update() acts as the safety net
            if (!_isMultiAttack) EchoDisappearEffect();
            return;
        }

        FlipToFace(_currentTarget);
        anim.SetBool("CanRun", true);
        float dirX = _currentTarget.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(dirX * _moveSpeed, rb.velocity.y);
    }

    #endregion






    #region Healing Echo Skill logic
    public void TimeEchoHealing(Transform spawnTransform = null)
    {
        SpawnTimeEcho(spawnTransform, UnityEngine.Random.Range(.5f, 1f)); // Example of random horizontal offset for visual variety
        // Implement the logic for the extra healing echo here.
        // This could involve creating a healing area, applying healing to the player, etc.
        Debug.Log("Extra Healing Echo Spawned!");
        //PLay the Healing VFX and SFX  and skill object animation here.
        if (_healingVfxPrefab != null)
        {
            float amount = _skillTimeEchoDefinition.HealingAmount;
            GameObject vfx = Instantiate(_healingVfxPrefab,spawnTransform.position,Quaternion.identity);
            vfx.transform.SetParent(spawnTransform); 
            EventBus<PlayerAddHealthAmount>.Raise(new PlayerAddHealthAmount(amount)); // Example of raising a health changed event to heal the player (adjust values as needed)
            Destroy(vfx, 2f); // Destroy the VFX after 2 seconds (adjust as needed)
        }
        EchoDisappearEffect(); // End the skill object after applying the healing effect
    }
    public void TimeEChoHealingAndCoolDownAllSkill(Transform spawnTransform = null)
    {
        float cdPercent = _skillTimeEchoDefinition.CDAmountPercent;
        Player player = spawnTransform.GetComponent<Player>();
        player.SkillButtonHandler.ReduceAllCooldowns(cdPercent);
        TimeEchoHealing(spawnTransform);
    }


    #endregion

    public override void AttackTrigger()
    {
        base.AttackTrigger();
        DealDamage(false);

        if (_isMultiAttack && Time.time < _multiAttackEndTime)
        {
            DetectEnemiesInRange(); // chain to next nearest enemy while duration allows
            return;
        }

        EchoDisappearEffect();
    }

    public override void AttackTrigger(bool canKnock)
    {
        base.AttackTrigger(canKnock);
        DealDamage(canKnock);
        EchoDisappearEffect();
    }

    private void DealDamage(bool appliesKnockback)
    {
        float damage = _skillTimeEchoDefinition.Damage;
        DamageEnemiesInRadius(damage, appliesKnockback);
    }

    private void SpawnSmokeEffect()
    {
        if (_onDeathVfxPrefab != null)
            PoolableVfx.Spawn(_onDeathVfxPrefab, transform.position);
    }

    


      private void EchoDisappearEffect()
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
        yield return new WaitForSeconds(1.5f);
        HandleDeath();
    }

   
    private void FlipToFace(Transform target)
    {
        float dirX = target.position.x - transform.position.x;
        Vector3 scale = transform.localScale;
        scale.x = dirX > 0f ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (_groundCheckPoint == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _dectectionRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * _stoppingDistance);
    }
    
    
    
}