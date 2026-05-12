using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Locks the enemy in place while the attack animation plays.
    /// Transitions to Chase or Idle once the animation triggers completion.
    /// </summary>
    public class EnemyAttackState : EnemyState
    {
        public EnemyAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName) {}

        public override void Enter()
        {
            base.Enter();
            if (enemy.ShouldEnemyRetreat())
            {
                enemy.Flip(enemy.direction);
                rb.velocity = new Vector2(enemy.enemyData.attackVelocityRetreat.x * -enemy.direction, enemy.enemyData.attackVelocityRetreat.y);
                return;
            }
            enemy.SetVelocity(new Vector2(0, rb.velocity.y));
        }

        public override void Update()
        {
            base.Update();
            enemy.FacePlayer();
            if (!isTriggered) return;

            if (enemy.IsPlayerDetected())
                stateMachine.ChangeState(enemy.enemyChaseState);
            else
                stateMachine.ChangeState(enemy.enemyIdleState);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
