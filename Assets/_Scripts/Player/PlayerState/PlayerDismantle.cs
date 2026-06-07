using UnityEngine;
using stateMachine;
using Base;

namespace player
{
    public class PlayerDismantleState : PlayerState
    {
        public PlayerDismantleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            stateTimer = ServiceLocator.Get<PlayerSkillManager>().skillDismantle.SkillBaseDefinition.Duration;
            ServiceLocator.Get<PlayerSkillManager>().skillDismantle.ExecuteSkillEffect();
            player.SetVelocity(new Vector2(0, rb.velocity.y));
            input.Disable();
        }

        public override void Update()
        {
            base.Update();
            if(stateTimer <= 0)
                stateMachine.ChangeState(player.playerIdleState);
        }

        public override void Exit()
        {
            base.Exit();
            input.Enable();
        }
    }
}