using System.Diagnostics;
using Base;
using stateMachine;

namespace player
{
    public class PlayerDomainExpasionState : PlayerState
    {
        public PlayerDomainExpasionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            ServiceLocator.Get<PlayerSkillManager>()?.domainExpasionSkill.ExecuteSkillEffect();
            DebugCustom.Log("This is Domain Expasion");
            stateTimer = ServiceLocator.Get<PlayerSkillManager>().domainExpasionSkill.SkillBaseDefinition.Duration;
            rb.simulated = false;
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
            rb.simulated = true;
            input.Enable();
        }
    }
}