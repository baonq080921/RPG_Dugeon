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

        protected override float MaxHealth => _enemy.enemyData.MaxHealth;

        /// <inheritdoc/>
        public override void TakeDamage(float damage, bool applyKnockBack)
        {
            base.TakeDamage(damage, applyKnockBack);
            GetComponent<IHitVFX>()?.PlayHitVFX();
            _enemy.stateMachine.ChangeState(_enemy.enemyStunState);
        }

        
    }
}
