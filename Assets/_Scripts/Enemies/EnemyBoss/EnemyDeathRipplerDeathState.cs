using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>Death state specific to <see cref="EnemyDeathRippler"/>.</summary>
    public class EnemyDeathRipplerDeathState : EnemyState
    {
        private EnemyDeathRippler _enemyDeathRippler;
        public EnemyDeathRipplerDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyDeathRippler = enemy as EnemyDeathRippler;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.SetVelocity(Vector2.zero);
            stateTimer = 2f;
            _enemyDeathRippler.UnTargetableEnemy(true);
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer <= 0)
            {
                stateMachine.ChangeState(_enemyDeathRippler.enemyIdleState);
                enemy.ReturnToPool();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _enemyDeathRippler.ResetDie();
        }
    }
}
