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

        protected override float MaxHealth => _player.Data.MaxHealth;

        /// <inheritdoc/>
        public override void TakeDamage(float damage, bool applyKnockBack)
        {
            base.TakeDamage(damage, applyKnockBack);
            if (!applyKnockBack) return;
            Vector2 knockBack = new Vector2(_player.Data.KnockBack.x * -_player.direction, _player.Data.KnockBack.y);
            _player.ReciveKnockBack(knockBack, _player.Data.KnockBackDuration);
            _player.stateMachine.ChangeState(_player.playerKnockBackState);
        }
    }
}
