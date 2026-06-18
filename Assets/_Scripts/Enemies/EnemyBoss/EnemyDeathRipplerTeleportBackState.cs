using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Teleports the boss to the opposite side of the player and immediately attacks.
    /// Triggered when <see cref="Enemy.ShouldEnemyRetreat"/> is true and the teleport roll succeeds.
    /// </summary>
    public class EnemyDeathRipplerTeleportBackState : EnemyState
    {
        private EnemyDeathRippler _enemyDeathRippler;

        public EnemyDeathRipplerTeleportBackState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyDeathRippler = enemy as EnemyDeathRippler;
        }

        public override void Enter()
        {
            base.Enter();
            stateTimer = 1.5f;
            _enemyDeathRippler.UnTargetableEnemy(true);
            _enemyDeathRippler.PrepareBehindPlayerTeleport();
            _enemyDeathRippler.SetTeleportTriggered(true);
        }

        public override void Update()
        {
            stateTimer -= Time.deltaTime;

            if (_enemyDeathRippler.IsTeleportTriggered)
            {
                enemy.transform.position = _enemyDeathRippler.FindTeleportPoint();
                _enemyDeathRippler.SetTeleportTriggered(false);
            }

            if (isTriggered || stateTimer < 0)
                stateMachine.ChangeState(_enemyDeathRippler.enemyAttackState);
        }

        public override void Exit()
        {
            base.Exit();
            _enemyDeathRippler.SetTeleportTriggered(false);
            _enemyDeathRippler.UnTargetableEnemy(false);
        }
    }
}
