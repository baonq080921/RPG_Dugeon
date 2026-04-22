using UnityEngine;
using Base;
using stateMachine;
namespace player
{
    public class PlayerDashState : EntityState
    {
        private float lastDashTime;
        private int _dashDirection;

        public PlayerDashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.SetCanDash(false);
            lastDashTime = Time.time;
            rb.gravityScale = 0f;
            _dashDirection = player.movementInput.x != 0 ? _dashDirection = (int) player.movementInput.x : _dashDirection = (int) player.direction;
            ApplyDashVelocity();
        }

        public override void Update()
        {
            base.Update();
            if (Time.time < lastDashTime + player.dashDuration)
                return;

            if (player.isGrounded)
            {
                if (player.movementInput.x != 0)
                    stateMachine.ChangeState(player.playerMovementState);
                else
                    stateMachine.ChangeState(player.playerIdleState);
            }
            else
            {
                stateMachine.ChangeState(player.playerFallState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            player.SetCanDash(true);
            rb.gravityScale = 3.5f;
            player.StartDashCooldown();
        }

          private void ApplyDashVelocity()
        {
            player.SetVelocity(new Vector2(_dashDirection * player.dashSpeed, 0f));
        }
    }
}