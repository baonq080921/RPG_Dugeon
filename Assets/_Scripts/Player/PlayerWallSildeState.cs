
using Base;
using UnityEngine;
namespace player{
    public class PlayerWallSildeState : EntityState
    {
        public PlayerWallSildeState(Player player, stateMachine.StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.ConsumeJump();
        }

        public override void Update()
        {
            base.Update();
            HandleWallSlide();

            if(player.JumpJustPressed)
            {
                stateMachine.ChangeState(player.playerWallJumpState);
                return;
            }
            if (!player.isTouchingWall)
                stateMachine.ChangeState(player.playerFallState);

            if (player.isGrounded)
            {
                stateMachine.ChangeState(player.playerIdleState);
                player.Flip(-player.direction);

            }
        }

        private void HandleWallSlide()
        {
            // When pressing down, let gravity accelerate the fall naturally
            if (player.movementInput.y >= 0f)
            {
                player.SetVelocity(new Vector2(rb.velocity.x, rb.velocity.y * player.slideDownSpeed));
            }
        }
    }
}