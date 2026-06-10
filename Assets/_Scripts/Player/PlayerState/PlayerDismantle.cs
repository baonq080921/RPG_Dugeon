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
            var dismantleSkill = ServiceLocator.Get<PlayerSkillManager>()?.skillDismantle;
            if (dismantleSkill == null) return;
            stateTimer = dismantleSkill.SkillBaseDefinition.Duration;
            dismantleSkill.ExecuteSkillEffect();
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