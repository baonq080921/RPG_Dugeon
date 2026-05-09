
using Base;
using stateMachine;
using UnityEngine;
namespace player
{
    public class PlayerFallState : PlayerAirBoneState
    {
        public PlayerFallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            if (stateMachine.previousState == player.playerDashState)
                rb.velocity = new Vector2(0, rb.velocity.y);
        }

        public override void Update()
        {
            base.Update();

            if (player.JumpJustPressed && player.JumpCount < player.MaxJumpCount)
            {
                stateMachine.ChangeState(player.playerJumpState);
                return;
            }

            if (player.isTouchingWall && !player.isGrounded && player.direction != player.LastWallJumpDirection)
            {
                stateMachine.ChangeState(player.playerWallSlideState);
                return;
            }

            if(player.isGrounded)
            {
                player.JumpCount = 0;
                player.LastWallJumpDirection = 0f;
                if(player.movementInput.x != 0)
                    stateMachine.ChangeState(player.playerMovementState);
                else
                    stateMachine.ChangeState(player.playerIdleState);
            }
        }

    }
}