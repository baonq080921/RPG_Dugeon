
using Base;
using stateMachine;

namespace player
{
    public class PlayerGroundedState : PlayerState
    {
        public PlayerGroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }


        public override void Enter()
        {
            base.Enter();
            player.ConsumeDash();
            // player.ConsumeAttack();
        }

        public override void Update()
        {
            base.Update();

            if (player.canDash && player.DashJustPressed)
            {
                stateMachine.ChangeState(player.playerDashState);
                return;
            }

            if (player.JumpJustPressed && player.canAttack && input.Player.BasicAttack.WasPressedThisFrame())
            {
                player.ConsumeJump();
                player.playerJumpAttackState.IsUpAttack = false;
                player.playerJumpAttackState.IsFromGround = true;
                stateMachine.ChangeState(player.playerJumpAttackState);
                return;
            }

            if(player.isJump)
                stateMachine.ChangeState(player.playerJumpState);
            if(!player.isGrounded)
                stateMachine.ChangeState(player.playerFallState);

            if (player.canAttack && input.Player.BasicAttack.WasPressedThisFrame())
                stateMachine.ChangeState(player.playerBasicAttackState);

            for (int i = 0; i < player.SkillManager.SlotCount; i++)
            {
                if (player.SkillManager.TryConsumeSkill(i, out PlayerState skillState))
                {
                    stateMachine.ChangeState(skillState);
                    return;
                }
            }
            
        }
    }
}