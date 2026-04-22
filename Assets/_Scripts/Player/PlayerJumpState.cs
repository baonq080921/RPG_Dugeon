
using Base;
using stateMachine;
using UnityEngine;
namespace player
{
    public class PlayerJumpState : PlayerAirBoneState
    {
        public PlayerJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.JumpCount++;
            player.ConsumeJump();
            player.SetVelocity(new Vector2(rb.velocity.x, player.jumpForce));
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

            if(rb.velocity.y < 0)
                stateMachine.ChangeState(player.playerFallState);
        }

    }
}