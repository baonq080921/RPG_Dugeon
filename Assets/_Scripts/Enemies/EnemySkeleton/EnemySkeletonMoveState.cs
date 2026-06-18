using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>Move state specific to <see cref="EnemySkeleton"/>.</summary>
    public class EnemySkeletonMoveState : EnemyState
    {
        public EnemySkeletonMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName) { }

        public override void Enter() => base.Enter();

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

            enemy.SetVelocity(new Vector2(enemy.direction * enemy.moveSpeed, rb.velocity.y));
        }

        public override void Exit() => base.Exit();
    }
}
