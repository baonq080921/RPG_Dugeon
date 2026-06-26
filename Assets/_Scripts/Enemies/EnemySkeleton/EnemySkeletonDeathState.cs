using UnityEngine;
using stateMachine;

namespace enemy
{
    public class EnemySkeletonDeathState : EnemyState
    {
        private EnemySkeleton _enemySkeleton;
        public EnemySkeletonDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
            _enemySkeleton = enemy as EnemySkeleton;
        }
        public override void Enter()
        {
            base.Enter();
            enemy.SetVelocity(new Vector2(3f,10f));
            stateTimer = 2f;
            _enemySkeleton.UnTargetableEnemy(true);
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer <= 0)
            {
                stateMachine.ChangeState(_enemySkeleton.enemyIdleState);
                _enemySkeleton.ReturnToPool();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _enemySkeleton.ResetDie();
        }
    }
}