using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>Idle state specific to <see cref="EnemyDeathRippler"/>.</summary>
    public class EnemyDeathRipplerTeleportState : EnemyGroundedState
    {
        private EnemyDeathRippler _enemyDeathRippler;
        public EnemyDeathRipplerTeleportState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyDeathRippler = enemy as EnemyDeathRippler;
        }

        public override void Enter()
        {
            base.Enter();
            stateTimer = 1.5f; // fallback in case animation event never fires
            _enemyDeathRippler.UnTargetableEnemy(true);
        }

        public override void Update()
        {
            stateTimer -= Time.deltaTime;
            if (_enemyDeathRippler.IsTeleportTriggered)
            {
                _enemyDeathRippler.transform.position = _enemyDeathRippler.FindTeleportPoint();
                _enemyDeathRippler.SetTeleportTriggered(false); // teleport once, not every frame
            }

            if (isTriggered || stateTimer < 0)
                stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerBattleState);
        }

        public override void Exit()
        {
            base.Exit();
            _enemyDeathRippler.SetTeleportTriggered(false);
            _enemyDeathRippler.UnTargetableEnemy(false);
        }
    }
}
