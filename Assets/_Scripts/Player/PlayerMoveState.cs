
using stateMachine;
using UnityEngine;
namespace player
{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();

            if (stateMachine.currentState != this) return;

            if(player.movementInput.x == 0 || player.isTouchingWall)
                stateMachine.ChangeState(player.playerIdleState);
            player.SetVelocity(new Vector2(player.movementInput.x * player.moveSpeed, rb.velocity.y));
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}