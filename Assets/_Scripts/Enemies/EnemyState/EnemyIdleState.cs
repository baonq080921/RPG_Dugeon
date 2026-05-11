
using Base;
using stateMachine;
using UnityEngine;
namespace enemy
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            stateTimer = enemy.enemyData.IdleTime;
            enemy.SetVelocity(new Vector2(0, rb.velocity.y));
        }

        public override void Update()
        {
            base.Update();

            if (enemy.IsPlayerDetected())
            {
                stateMachine.ChangeState(enemy.enemyChaseState);
                return;
            }

            if (stateTimer <= 0)
                stateMachine.ChangeState(enemy.enemyMoveState);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}