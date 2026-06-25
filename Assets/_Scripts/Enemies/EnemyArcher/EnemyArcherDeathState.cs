using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>Death state specific to <see cref="EnemyDeathRippler"/>.</summary>
    public class EnemyArcherDeathState : EnemyState
    {
        private EnemyArcher _enemyArcher;
        public EnemyArcherDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyArcher = enemy as EnemyArcher;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.col.enabled = false;
            enemy.SetVelocity(Vector2.zero);
            stateTimer = 2f;
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer <= 0)
            {
                stateMachine.ChangeState(_enemyArcher.enemyIdleState);
                _enemyArcher.ReturnToPool();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _enemyArcher.ResetDie();

        }
    }
}
