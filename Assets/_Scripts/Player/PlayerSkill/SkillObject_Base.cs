using Base;
using entity;
using Interfaces;
using UnityEngine;

namespace player
{
    /// <summary>
    /// Base class for all spawned skill objects. Provides common references and a ground check.
    /// </summary>
    public class SkillObject_Base : MonoBehaviour
    {
        [SerializeField] protected Rigidbody2D rb;
        [SerializeField] protected Animator anim;


        /// <summary>True when the object is resting on ground.</summary>
        [Header("Attack Check")]

        [SerializeField] private float _attackRadius = 0.5f;
        [SerializeField] protected LayerMask _enemyLayer;
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private bool _isDraw = true;
        protected virtual void Awake()
        {
            if (rb == null)
                rb = GetComponentInChildren<Rigidbody2D>();
            if (anim == null)
                anim = GetComponentInChildren<Animator>();
        }

        protected virtual void Update() { }


        public void DamageEnemiesInRadius(float damage, bool isCrit, ElementType elementType = ElementType.None, bool appliesKnockback = true)
        {
            var cols = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRadius, _enemyLayer);
            foreach (var col in cols)
            {
                var targetVfx = col.GetComponent<EntityVfx>();
                if (col.TryGetComponent<IHit>(out var hit))
                {
                    Transform target = appliesKnockback ? col.transform : null;
                    hit?.TakeDamage(damage, 0, ElementType.None, target);
                    targetVfx.UpdateStatusEffectVFX(elementType, 0.3f);
                    // Spawn the popup at the hit collider, not at target, which is null when knockback is skipped.
                    EventBus<DamagePopupEvent>.Raise(new DamagePopupEvent(col.transform.position, damage, isCrit));
                }
            }
        }


        public virtual void HandleDeath() { }
        protected virtual void OnDrawGizmos()
        {
            if (!_isDraw) return;
            if (_attackPoint == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
        }

        public virtual void AttackTrigger() { }
        public virtual void AttackTrigger(bool canKnock) { }
    }
}
