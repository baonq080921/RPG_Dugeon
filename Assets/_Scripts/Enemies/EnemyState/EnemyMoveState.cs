
using Base;
using stateMachine;
using UnityEngine;
namespace enemy
{
    public class EnemyMoveState : EnemyState
    {
        public EnemyMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }


        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();

            if (enemy.IsPlayerDetected())
            {
                stateMachine.ChangeState(enemy.enemyChaseState);
                return;
            }

            if (!enemy.isGrounded || enemy.isTouchingWall)
            {
                enemy.SetDirection(-enemy.direction);
                enemy.Flip(enemy.direction);
                stateMachine.ChangeState(enemy.enemyIdleState);
                return;
            }

            enemy.SetVelocity(new Vector2(enemy.direction * enemy.enemyData.MoveSpeed, rb.velocity.y));
        }


        public override void Exit()
        {
            base.Exit();
        }
    }
}