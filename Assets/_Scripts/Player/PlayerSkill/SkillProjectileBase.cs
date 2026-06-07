using System;
using System.Collections;
using Interfaces;
using UnityEngine;

/// <summary>
/// Generic pooled projectile base. Works for both player and enemy projectiles.
/// Set <see cref="_hitLayer"/> in the Inspector to control what the projectile can hit
/// (enemy layer for player projectiles, player layer for enemy projectiles).
/// Subclasses override <see cref="OnHit"/> to apply damage or effects on contact.
/// </summary>
public abstract class SkillProjectileBase : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private LayerMask _hitLayer;

    private Action _onRelease;
    private Coroutine _lifetimeCoroutine;

    protected float DirectionX;
    protected float FlySpeed { get; private set; }

    /// <summary>When true the projectile returns to the pool on the first hit. Override and return false for pass-through.</summary>
    protected virtual bool DestroyOnHit => true;

    protected virtual void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>Called by the pool after renting this instance. Wires up the release callback.</summary>
    public void Init(Action onRelease) => _onRelease = onRelease;

    /// <summary>
    /// Positions the projectile at <paramref name="origin"/>, reads facing direction from its
    /// localScale, then starts the lifetime timer.
    /// </summary>
    protected void Launch(Transform origin, float speed, float lifetime)
    {
        transform.position = origin.position;
        DirectionX = origin.localScale.x > 0f ? 1f : -1f;
        FlySpeed = speed;

        if (_lifetimeCoroutine != null)
            StopCoroutine(_lifetimeCoroutine);
        _lifetimeCoroutine = StartCoroutine(LifetimeRoutine(lifetime));
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(DirectionX * FlySpeed, rb.velocity.y);
        OnFixedUpdate();
    }

    /// <summary>Called every physics tick after velocity is applied. Override for return-tracking or other per-tick logic.</summary>
    protected virtual void OnFixedUpdate() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_hitLayer.value & (1 << other.gameObject.layer)) == 0) return;
        OnHit(other);
        if (DestroyOnHit) HandleDeath();
    }

    /// <summary>
    /// Override to apply damage or effects when this projectile hits a valid target.
    /// Use <c>other.GetComponent&lt;IHit&gt;()</c> to damage either enemy or player uniformly.
    /// </summary>
    protected virtual void OnHit(Collider2D other) { }

    /// <summary>Returns the projectile to the pool. Safe to call from multiple paths.</summary>
    public virtual void HandleDeath()
    {
        if (_onRelease == null) return;
        if (_lifetimeCoroutine != null)
        {
            StopCoroutine(_lifetimeCoroutine);
            _lifetimeCoroutine = null;
        }
        var release = _onRelease;
        _onRelease = null;
        release.Invoke();
    }

    private IEnumerator LifetimeRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        HandleDeath();
    }
}
