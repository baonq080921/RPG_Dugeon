using stateMachine;
using UnityEngine;

namespace enemy
{
    public class EnemyArcherIdleState : EnemyState
    {
        private EnemyArcher _enemyArcher;
        public EnemyArcherIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
            _enemyArcher = enemy as EnemyArcher;
        }


        public override void Enter()
        {
            base.Enter();
            _enemyArcher.SetVelocity(Vector2.zero);
        }


        public override void Update()
        {
            base.Update();
            if (_enemyArcher.IsPlayerDetected())
                stateMachine.ChangeState(_enemyArcher.enemyMoveState);

        }

        public override void Exit()
        {
            base.Exit();
        }


    }
}
