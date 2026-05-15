using System.Collections.Generic;
using System.Linq;
using Base;
using Interfaces;
using UnityEngine;

/// <summary>
/// Handles hit detection and damage application for any entity.
/// Call <see cref="PerformedAttack"/> from an animation event at the moment an attack lands.
/// </summary>
public class EntityCombat : MonoBehaviour
{
    [SerializeField] protected List<Collider2D> targetColliders;
    [SerializeField] private Transform _targetCheck;
    [SerializeField] protected float _targetRadius = 0.75f;
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private Color _hitColor;
    protected Entity entity;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
    }

    /// <summary>
    /// Detects all targets in range and applies damage to each one that implements <see cref="IHit"/>.
    /// </summary>
    public void PerformedAttack()
    {
        DetectTargetColliders();
        foreach (var target in targetColliders)
        {
            if (target.TryGetComponent<IHit>(out var hit))
                hit?.TakeDamage(entity.Damage, entity.CanKnockBackOnHit);
                CreateHitEffect(target.GetComponent<Transform>());
        }
    }


    private void CreateHitEffect(Transform target)
    {
        float randomX = Random.Range(-0.3f,0.3f);
        float randomY = Random.Range(-0.7f,0.7f);
        Vector2 randomHitOffSet = new Vector2(randomX,randomY);
        ServiceLocator.Get<HitEffectPool>().SpawnHitEffect(target,_hitColor, randomHitOffSet);
    }

    protected void DetectTargetColliders()
    {
        if (targetColliders != null) targetColliders.Clear();
        targetColliders = Physics2D.OverlapCircleAll(_targetCheck.position, _targetRadius, entity.LayerMask).ToList();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_targetCheck.position, _targetRadius);
    }
}
