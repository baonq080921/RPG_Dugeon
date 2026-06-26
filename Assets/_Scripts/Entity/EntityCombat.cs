using System.Collections.Generic;
using System.Linq;
using Base;
using Interfaces;
using UnityEngine;

namespace entity
{
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
        // public event Action<Transform,bool> OnTargetHit;

        [Range(0f, 2f)]
        [SerializeField] private float _scaleElementalFactor = 1f;

        [SerializeField] private float _electricStatusDuration = 2f;

        [Range(0f, 1f)]
        [SerializeField] private float _electricBuildUpCharge = 0.5f;

        [SerializeField] private Vector3 _damagePopupOffset = new Vector3(0f, 1f, 0f);

        protected virtual void Awake()
        {
            entity = GetComponent<Entity>();
        }

        /// <summary>
        /// Detects all targets in range and applies damage to each one that implements <see cref="IHit"/>.
        /// </summary>
        public virtual void PerformedAttack()
        {
            IsdetectTargetColliders();
            foreach (var target in targetColliders)
            {
                IHit hit = target.GetComponent<IHit>();
                if (hit == null) continue;
                Transform targetTakeDamage = target.GetComponent<Transform>();
                float physicalDamage = entity.entityStat.GetPhysicalDamageValue(out bool isCrit);
                float elementalDamage = entity.entityStat.GetElementalDamageValue(out ElementType elementType);
                // Debug.Log(physicalDamage+ "physics");
                // Debug.Log(elementalDamage+ "magic");
                bool targetGotHit = hit.TakeDamage(physicalDamage, elementalDamage, elementType, targetTakeDamage);
                if (elementType != ElementType.None)
                    ApplyStatusEffect(elementType, targetTakeDamage, _scaleElementalFactor);
                if (targetGotHit)
                {
                    var hitColor = entity.entityVfx.GetHitColor(elementType);
                    var targetVFX = targetTakeDamage.GetComponent<EntityVfx>();
                    // OnTargetHit?.Invoke(target.transform,isCrit);
                    targetVFX.SpawnHitEffect(targetTakeDamage, isCrit, hitColor);
                    EventBus<DamagePopupEvent>.Raise(new DamagePopupEvent(target.transform.position, physicalDamage + elementalDamage, isCrit));
                }

            }
        }


        public void PerformedAttackOnTarget(Transform target, float damageScale = 1f)
        {

            IHit hit = target.GetComponent<IHit>();
            if (hit == null) return;
            Transform targetTakeDamage = target.GetComponent<Transform>();
            float physicalDamage = entity.entityStat.GetPhysicalDamageValue(out bool isCrit) * damageScale;
            float elementalDamage = entity.entityStat.GetElementalDamageValue(out ElementType elementType) * damageScale;
            // Debug.Log(physicalDamage+ "physics");
            // Debug.Log(elementalDamage+ "magic");
            bool targetGotHit = hit.TakeDamage(physicalDamage, elementalDamage, elementType, targetTakeDamage);
            if (elementType != ElementType.None)
                ApplyStatusEffect(elementType, targetTakeDamage, _scaleElementalFactor * damageScale);
            if (targetGotHit)
            {
                var hitColor = entity.entityVfx.GetHitColor(elementType);
                var targetVFX = targetTakeDamage.GetComponent<EntityVfx>();
                // OnTargetHit?.Invoke(target.transform,isCrit);
                targetVFX.SpawnHitEffect(targetTakeDamage, isCrit, hitColor);
                EventBus<DamagePopupEvent>.Raise(new DamagePopupEvent(target.transform.position, physicalDamage + elementalDamage, isCrit));
            }
        }




        public virtual void ApplyStatusEffect(ElementType elementType, Transform target, float scaleFactor = 1f)
        {
            EntityStatusHandler statusHandler = target.GetComponent<EntityStatusHandler>();
            if (statusHandler == null) return;

            if (elementType == ElementType.Electric && statusHandler.CanElementalStatusApply(elementType))
            {
                float electricDamage = entity.entityStat.GetElementalDamageValue(out _) * scaleFactor;
                statusHandler.ApplyElectricEffect(_electricStatusDuration, electricDamage, _electricBuildUpCharge); // Example duration
            }

            if (elementType == ElementType.Ice && statusHandler.CanElementalStatusApply(elementType))
            {
                statusHandler.ApplyIceEffect(elementType, scaleFactor);
            }

        }




        protected bool IsdetectTargetColliders()
        {
            if (targetColliders != null) targetColliders.Clear();
            targetColliders = Physics2D.OverlapCircleAll(_targetCheck.position, _targetRadius, entity.LayerMask).ToList();
            return targetColliders != null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(_targetCheck.position, _targetRadius);
        }
    }
}
