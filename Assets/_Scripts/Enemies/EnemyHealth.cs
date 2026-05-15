using Interfaces;
using UnityEngine;

namespace enemy
{
    /// <inheritdoc/>
    public class EnemyHealth : EntityHealth
    {
        private Enemy _enemy;

        protected override void Awake()
        {
            base.Awake();
            _enemy = GetComponent<Enemy>();
        }

        /// <inheritdoc/>
        public override bool TakeDamage(float damage, bool applyKnockBack ,Transform target)
        {
            bool canDamage = base.TakeDamage(damage, applyKnockBack, target);
            if (!canDamage) return false;
            _enemy.stateMachine.ChangeState(_enemy.enemyStunState);
            return true;
        }

        
    }
}
