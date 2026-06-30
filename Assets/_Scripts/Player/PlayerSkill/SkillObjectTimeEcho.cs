using System;
using System.Collections;
using Base;
using Interfaces;
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
    [SerializeField] private PoolableVfx _onDeathVfxPrefab;
    [Range(0, 10f)]
    [SerializeField] private float _dectectionRange = 5f;
    [SerializeField] private float _stoppingDistance = 0.5f;
    [SerializeField] private float _followStopDistance = 1f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _maxHealth = 100f;

    private float _currentHealth;
    private Coroutine _deathCoroutine;
    private Transform _currentTarget;
    private Transform _ownerTransform;
    private bool _isMultiAttack;
    private Action _onRelease;
    private float _sideKickLifeTime;

    public void Init(Action onRelease) => _onRelease = onRelease;

    public void ResetObject()
    {
        _currentHealth = _maxHealth;
        _currentTarget = null;
        _ownerTransform = null;
        _isMultiAttack = false;
        if (_deathCoroutine != null)
        {
            StopCoroutine(_deathCoroutine);
            _deathCoroutine = null;
        }
        if (rb != null) rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }
    public override void HandleDeath()
    {
        if (_onRelease == null) return; // already released — second concurrent call (timer + TakeDamage race)
        SpawnSmokeEffect();
        ResetObject();
        var release = _onRelease;
        _onRelease = null; // null before invoke so any re-entrant call hits the guard above
        release.Invoke();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (anim != null)
            anim.SetFloat("yVelocity", IsGrounded ? 1f : rb.velocity.y);

        if (_groundCheckPoint != null)
            IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _whatIsGround);

        if (_isMultiAttack)
        {
            if (_currentTarget != null)
                PursueTarget();
            else
                ScanAndFollow();
            return;
        }

        if (_currentTarget != null)
            PursueTarget();
    }

    public void SpawnTimeEcho(Transform spawnTransform = null, float offSetX = 0f)
    {
        _sideKickLifeTime = _skillTimeEchoDefinition.Duration;
        transform.position = spawnTransform != null ? spawnTransform.position : Vector3.zero;
        transform.position += new Vector3(UnityEngine.Random.Range(-offSetX, offSetX), 0f, transform.position.z);
    }

    public void TimeEchoBase(Transform spawnTransform = null)
    {
        SpawnTimeEcho(spawnTransform, UnityEngine.Random.Range(0.5f, 1.5f));
        EchoDisappearEffect();
    }

    #region Side Kick Attack Skill logic

    public void TimeEchoSideKickAttack(Transform spawnTransform = null, float damage = 0f)
    {
        SpawnTimeEcho(spawnTransform);
        _isMultiAttack = true;
        DetectEnemiesInRange();
    }

    public void TimeEchoSideKickMahoragaAttack(Transform spawnTransform = null, float damage = 0f)
    {
        SpawnTimeEcho(spawnTransform);
        _ownerTransform = spawnTransform;
        _isMultiAttack = true;
        EchoDisappearEffect();
    }

    // Single-attack only: disappears immediately if no enemies in range.
    private void DetectEnemiesInRange()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _dectectionRange, _enemyLayer);
        if (hits.Length == 0)
        {
            EchoDisappearEffect();
            return;
        }
        _currentTarget = GetNearestTarget(hits);
    }

    // Mahoraga: scan for enemies; if none found, follow the player instead.
    // Switches to pursuing as soon as an enemy enters range.
    private void ScanAndFollow()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _dectectionRange, _enemyLayer);
        if (hits.Length > 0)
        {
            anim.SetBool("CanRun", false);
            rb.velocity = new Vector2(0f, rb.velocity.y);
            _currentTarget = GetNearestTarget(hits);
            FlipToFace(_currentTarget);

            return;
        }

        if (_ownerTransform == null)
        {
            anim.SetBool("CanRun", false);
            return;
        }

        float distToOwner = Vector2.Distance(transform.position, _ownerTransform.position);
        if (distToOwner > _followStopDistance)
        {
            FlipToFace(_ownerTransform);
            anim.SetBool("CanRun", true);
            float dirX = _ownerTransform.position.x > transform.position.x ? 1f : -1f;
            rb.velocity = new Vector2(dirX * _moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            anim.SetBool("CanRun", false);
        }
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
        if (!IsGrounded)
        {
            anim.SetBool("CanRun", false);
            HandleDeath();
            return;
        }

        float dist = Vector2.Distance(transform.position, _currentTarget.position);
        if (dist <= _stoppingDistance)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            anim.SetBool("CanRun", false);
            anim.SetTrigger("Attack");
            _currentTarget = null;
            // if (!_isMultiAttack) EchoDisappearEffect();
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
        SpawnTimeEcho(spawnTransform, UnityEngine.Random.Range(.5f, 1f));
        if (_healingVfxPrefab != null)
        {
            float amount = _skillTimeEchoDefinition.HealingAmount;
            GameObject vfx = Instantiate(_healingVfxPrefab, spawnTransform.position, Quaternion.identity);
            vfx.transform.SetParent(spawnTransform);
            EventBus<PlayerAddHealthAmountEvent>.Raise(new PlayerAddHealthAmountEvent(amount));
            Destroy(vfx, 2f);
        }
        EchoDisappearEffect();
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
        EchoDisappearEffect();
    }

    public override void AttackTrigger(bool canKnock)
    {
        base.AttackTrigger(canKnock);
        DealDamage(canKnock);
        if (_isMultiAttack)
        {
            _currentTarget = null; // ScanAndFollow() picks up next enemy on next Update tick
            return;
        }

        EchoDisappearEffect();
    }

    private void DealDamage(bool appliesKnockback)
    {
        float skilldamage = _skillTimeEchoDefinition.Damage;
        float playerPhysicDamage = ServiceLocator.Get<Player>().entityStat.GetPhysicalDamageValue( out bool isCrit);
        float playerElementalDamage = ServiceLocator.Get<Player>().entityStat.GetPhysicalDamageValue( out _);
        float totalPlayerSkillDamage = playerPhysicDamage + playerElementalDamage;
        float finalDamage = skilldamage + totalPlayerSkillDamage * 0.8f;
        DamageEnemiesInRadius(finalDamage,isCrit, appliesKnockback);
    }

    private void SpawnSmokeEffect()
    {
        if (_onDeathVfxPrefab != null)
            PoolableVfx.Spawn(_onDeathVfxPrefab, transform.position);
    }

    private void EchoDisappearEffect()
    {
        if (_deathCoroutine != null)
            StopCoroutine(_deathCoroutine);
        _deathCoroutine = StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(_sideKickLifeTime);
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
