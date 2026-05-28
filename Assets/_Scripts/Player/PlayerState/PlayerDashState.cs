using UnityEngine;
using Base;
using stateMachine;
namespace player
{
    public class PlayerDashState : PlayerState
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
            stateTimer = player.SkillButtonHandler.GetSkill((int)ButtonSkillName.Dash).Duration;
            rb.gravityScale = 0f;
            _dashDirection = player.movementInput.x != 0 ? (int)Mathf.Sign(player.movementInput.x) : (int)player.direction;
            ApplyDashVelocity();
            player.AfterImageEffect?.StartEffect();
        }

        public override void Update()
        {
            base.Update();
            if(stateTimer >= 0) return;

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
            player.AfterImageEffect?.StopEffect();
            player.SetCanDash(true);
        }

          private void ApplyDashVelocity()
        {
            player.SetVelocity(new Vector2(_dashDirection * player.dashSpeed, 0f));
        }
    }
}