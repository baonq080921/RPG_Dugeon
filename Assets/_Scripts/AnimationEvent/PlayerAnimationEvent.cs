using Base;
using UnityEngine;

namespace player
{
    public class PlayerAnimationEvent : MonoBehaviour
    {
        private Player _player;
        void Awake()
        {
            _player = GetComponentInParent<Player>();
        }
        public void SetTrigger()
        {
            _player.playerBasicAttackState.TriggerAnimation();
        }

        public void SetTriggerFallAnimation() => _player.playerJumpAttackState.Trigger();
    }
}