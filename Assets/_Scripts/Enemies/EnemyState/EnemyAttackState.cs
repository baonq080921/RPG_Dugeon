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
            enemy.SetVelocity(new Vector2(enemy.direction * enemy.enemyData.attackVelocity.x, enemy.enemyData.attackVelocity.y));
        }

        public override void Update()
        {
            base.Update();
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
