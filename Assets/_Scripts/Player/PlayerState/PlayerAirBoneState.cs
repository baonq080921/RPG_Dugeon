
using stateMachine;
using UnityEngine;
namespace player
{
    public class PlayerAirBoneState : PlayerState
    {
        public PlayerAirBoneState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName){}

        public override void Enter()
        {
            base.Enter();
            player.ConsumeDash();
        }
        public override void Update()
        {
            base.Update();
            if(input.Player.BasicAttack.WasPressedThisFrame())
            {
                player.playerJumpAttackState.IsUpAttack = player.movementInput.y > 0;
                player.playerJumpAttackState.IsFromGround = false;
                stateMachine.ChangeState(player.playerJumpAttackState);
            }

            if(player.canDash && player.DashJustPressed)
            {
                stateMachine.ChangeState(player.playerDashState);
                return;
            }
            if(player.movementInput.x != 0)
                 player.SetVelocity(new Vector2(player.movementInput.x * player.moveSpeed * player.coyoteTime, rb.velocity.y));
        }
    }
}