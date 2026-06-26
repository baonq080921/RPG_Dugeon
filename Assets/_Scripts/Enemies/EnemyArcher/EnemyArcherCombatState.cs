using UnityEngine;
using stateMachine;

namespace enemy
{
    public class EnemyArcherCombatState : EnemyState
    {

        private EnemyArcher _enemyArcher;
        public EnemyArcherCombatState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
            _enemyArcher = enemy as EnemyArcher;
        }

        public override void Enter()
        {
            base.Enter();
            _enemyArcher.SetVelocity(Vector2.zero);
            _enemyArcher.FacePlayer();
        }

        public override void Update()
        {
            base.Update();
            if (_enemyArcher.IsInAttackRange())
            {
                if (_enemyArcher.CanAttack())
                {

                    stateMachine.ChangeState(_enemyArcher.enemyAttackState);
                    return;
                }
                else
                {
                    stateMachine.ChangeState(_enemyArcher.enemyIdleState);
                    return;
                }
            }
            else
                stateMachine.ChangeState(_enemyArcher.enemyMoveState);


        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}
