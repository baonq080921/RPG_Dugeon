using stateMachine;
using UnityEngine;
namespace player
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName){}


        public override void Enter()
        {
            base.Enter();
            player.SetVelocity(new Vector2(0, player.rb.velocity.y));
        }

        public override void Update()
        {
            base.Update();

            if (stateMachine.currentState != this) return;
            bool isPressingIntoWall = player.isTouchingWall && Mathf.Sign(player.movementInput.x) == player.direction;
            if(player.movementInput.x != 0 && !isPressingIntoWall)
                stateMachine.ChangeState(player.playerMovementState);
        }

        
        public override void Exit()
        {
            base.Exit();
        }
    }
}