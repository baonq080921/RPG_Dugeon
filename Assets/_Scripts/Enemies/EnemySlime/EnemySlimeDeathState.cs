using UnityEngine;
using stateMachine;

namespace enemy
{
    public class EnemySlimeDeathState : EnemyState
    {
        private EnemySlime _enemySlime;
        
        public EnemySlimeDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
            _enemySlime = enemy as EnemySlime;
        }
        public override void Enter()
        {
            base.Enter();
            _enemySlime.CreateChild();
            stateTimer = 2f;
            _enemySlime.UnTargetableEnemy(true);
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer <= 0)
            {                
                stateMachine.ChangeState(_enemySlime.enemyIdleState);
                _enemySlime.ReturnToPool();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _enemySlime.ResetDie();
        }
    }
}