using UnityEngine;
using stateMachine;
namespace player
{
    public class PlayerWallJumpState : PlayerAirBoneState
    {
        public PlayerWallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            float wallDirection = player.direction;
            player.LastWallJumpDirection = wallDirection;
            player.JumpCount = 0;
            player.ConsumeJump();
            player.SetVelocity(new Vector2(player.WallJumpForce.x * -wallDirection, player.WallJumpForce.y));
            player.HandleFlip(-wallDirection);
        }

        public override void Update()
        {
            base.Update();

            if (player.isGrounded)
            {
                player.JumpCount = 0;
                player.LastWallJumpDirection = 0f;
                if (player.movementInput.x != 0)
                    stateMachine.ChangeState(player.playerMovementState);
                else
                    stateMachine.ChangeState(player.playerIdleState);
                return;
            }

            // Only check wall/fall transitions once the upward arc is done
            if (rb.velocity.y < 0)
            {
                if (player.isTouchingWall && player.direction != player.LastWallJumpDirection)
                    stateMachine.ChangeState(player.playerWallSlideState);
                else
                    stateMachine.ChangeState(player.playerFallState);
            }
        }

    }
}