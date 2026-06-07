using Base;
using Interfaces;
using player;
using UnityEngine;

public class SkillObjectDismantle : SkillProjectileBase
{
    [SerializeField] private SkillBaseDefinition _skillBaseDefinition;

    private bool _willReturnOnHit;
    private bool _isReturning;
    private Transform _ownerTransform;

    protected override bool DestroyOnHit => false;

    /// <summary>Base dismantle: flies in one direction and expires after duration.</summary>
    public void Dismantle(Transform tf)
    {
        ResetState();
        _ownerTransform = tf;
        Launch(tf, _skillBaseDefinition.moveSpeed, _skillBaseDefinition.Cooldown);
    }

    /// <summary>Upgraded dismantle: flips back toward the owner on the first enemy hit.</summary>
    public void DismantleWithReturn(Transform tf)
    {
        ResetState();
        _willReturnOnHit = true;
        _ownerTransform = tf;
        Launch(tf, _skillBaseDefinition.moveSpeed, _skillBaseDefinition.Cooldown);
    }

    protected override void OnHit(Collider2D other)
    {
        if (other.TryGetComponent<IHit>(out var target))
            target.TakeDamage(_skillBaseDefinition.Damage, 0, ElementType.None, transform);

        other.GetComponent<IHitVFX>()?.PlayHitVFX();
        EventBus<TargetGotHitEvent>.Raise(new TargetGotHitEvent(other.transform, false));

        if (_willReturnOnHit && !_isReturning)
            FlipDirection();
    }

    protected override void OnFixedUpdate()
    {
        if (!_isReturning || _ownerTransform == null) return;

        bool passedOwner = DirectionX < 0f
            ? transform.position.x <= _ownerTransform.position.x
            : transform.position.x >= _ownerTransform.position.x;

        if (passedOwner) HandleDeath();
    }

    private void FlipDirection()
    {
        _isReturning = true;
        DirectionX *= -1f;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void ResetState()
    {
        _willReturnOnHit = false;
        _isReturning = false;
        _ownerTransform = null;
    }
}
