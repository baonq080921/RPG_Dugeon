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
    protected Entity entity;
    protected EntityStat entityStat;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityStat = GetComponent<EntityStat>();
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
                hit?.TakeDamage(10, entity.CanKnockBackOnHit, target.transform);
        }
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
