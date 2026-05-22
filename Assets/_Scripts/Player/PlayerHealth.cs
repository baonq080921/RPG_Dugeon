using UnityEngine;

namespace player
{
    /// <inheritdoc/>
    public class PlayerHealth : EntityHealth
    {
        private Player _player;

        protected override void Awake()
        {
            base.Awake();
            _player = GetComponent<Player>();
            InvokeRepeating(nameof(RegenerateHealth), 0f, 1f); // Regenerate health every second
        }



        /// <inheritdoc/>
        public override bool TakeDamage(float damage,float elementalDamage,ElementType elementType,Transform target)
        {
            bool canDamage = base.TakeDamage(damage,elementalDamage,elementType,target);
            if (!canDamage) return false;   
            if(_player.isDead) return false;

            _player.ApplyKnockBack(damage);
            _player.stateMachine.ChangeState(_player.playerKnockBackState);
            return true;
        }
    }
}
