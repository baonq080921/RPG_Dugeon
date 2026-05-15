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
        }
        /// <inheritdoc/>
        public override bool TakeDamage(float damage, bool applyKnockBack, Transform target)
        {
            bool canDamage = base.TakeDamage(damage, applyKnockBack,target);
            if (!canDamage) return false;

            if (!applyKnockBack) return false;


            Vector2 knockBack = new Vector2(_player.Data.KnockBack.x * -_player.direction, _player.Data.KnockBack.y);
            _player.ReciveKnockBack(knockBack, _player.Data.KnockBackDuration);
            _player.stateMachine.ChangeState(_player.playerKnockBackState);

            return true;
        }
    }
}
