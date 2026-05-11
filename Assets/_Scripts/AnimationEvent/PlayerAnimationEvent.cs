using Base;
using UnityEngine;

namespace player
{
    public class PlayerAnimationEvent : EntityAnimationEvent
    {
        private Player _player;
        protected override void Awake()
        {
            base.Awake();
            _player = GetComponentInParent<Player>();
        }

        public void SetTriggerFallAnimation() => _player.playerJumpAttackState.TriggerFallState();
    }
}