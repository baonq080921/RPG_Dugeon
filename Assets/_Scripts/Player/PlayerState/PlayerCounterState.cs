using UnityEngine;
using stateMachine;

namespace player
{
    public class PlayerCounterState : PlayerState
    {
        private PlayerCombat _playerCombat;
        private bool isCounter;
        public PlayerCounterState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
            _playerCombat = player.GetComponent<PlayerCombat>();
        }

        public override void Enter()
        {
            base.Enter();
            stateTimer = player.SkillManager.GetDefinition((int)SkillName.CounterSkill).Duration;
            isCounter = _playerCombat.IsPerformedCounter();
            animator.SetBool("CanCounter", isCounter);
            player.SetVelocity(new Vector2(0, rb.velocity.y));

        }

        public override void Update()
        {
            base.Update();

            if (isTriggered)
                stateMachine.ChangeState(player.playerIdleState);

            if (stateTimer <= 0 && !isCounter)
                stateMachine.ChangeState(player.playerIdleState);
        }

        public override void Exit()
        {
            base.Exit();
            animator.SetBool("CanCounter", false);
        }
    }
}