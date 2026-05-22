using System;
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
    private EntityVfx _entityVfx;
    public event Action<Transform,bool> OnTargetHit;
    
    [Range(0f,2f)]
    [SerializeField] private float _scaleElementalFactor=1f;

    [SerializeField] private float _electricStatusDuration = 2f;
    
    [Range(0f,1f)]
    [SerializeField] private float _electricBuildUpCharge = 0.5f;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityStat = GetComponent<EntityStat>();
        _entityVfx = GetComponentInChildren<EntityVfx>();
    }

    /// <summary>
    /// Detects all targets in range and applies damage to each one that implements <see cref="IHit"/>.
    /// </summary>
    public void PerformedAttack()
    {
        DetectTargetColliders();
        foreach (var target in targetColliders)
        {
            IHit hit = target.GetComponent<IHit>();
            EntityStat targetStat = target.GetComponent<EntityStat>();
            if(hit == null) continue;
            if(targetStat == null) continue;
            Transform damageDealer = target.GetComponent<Transform>();
            float physicalDamage = entityStat.GetPhysicalDamageValue(out bool isCrit);
            float elementalDamage = entityStat.GetElementalDamageValue(out ElementType elementType);
           
            bool targetGotHit = hit.TakeDamage(physicalDamage,elementalDamage,elementType, damageDealer);
            if(elementType != ElementType.None)
                ApplyStatusEffect(elementType, damageDealer,_scaleElementalFactor);
            if (targetGotHit)
            {
                _entityVfx?.UpdateHitColor(elementType);
                OnTargetHit?.Invoke(target.transform,isCrit);
            }

        }
    }
    

    public void ApplyStatusEffect(ElementType elementType, Transform target, float scaleFactor = 1f)
    {
        EntityStatusHandler statusHandler = target.GetComponent<EntityStatusHandler>();
        if (statusHandler == null) return;

        if(elementType == ElementType.Electric && statusHandler.CanElementalStatusApply(elementType))
        {
            float electricDamage = entityStat.GetElementalDamageValue(out _) * scaleFactor;
            statusHandler.ApplyElectricEffect(_electricStatusDuration,electricDamage,_electricBuildUpCharge); // Example duration
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
