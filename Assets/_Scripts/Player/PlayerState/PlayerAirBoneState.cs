
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
        }
        public override void Update()
        {
            base.Update();
            if (player.canAirAttack && input.Player.BasicAttack.WasPressedThisFrame())
            {
                player.playerJumpAttackState.IsUpAttack = player.movementInput.y > 0;
                player.playerJumpAttackState.IsFromGround = false;
                stateMachine.ChangeState(player.playerJumpAttackState);
                return;
            }

            if (player.SkillButtonHandler.TryConsumeSkill((int)ButtonSkillName.Dash, out PlayerState dashState))
            {
                stateMachine.ChangeState(dashState);
                return;
            }
            if(player.movementInput.x != 0)
                player.SetVelocity(new Vector2(player.movementInput.x * player.moveSpeed * player.airControlFactor, rb.velocity.y));
        }
    }
}