using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Attack state specific to <see cref="EnemySlime"/>.
    /// Locks the enemy in place while the attack animation plays.
    /// </summary>
    public class EnemySlimeAttackState : EnemyState
    {
        public EnemySlimeAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            if (enemy.ShouldEnemyRetreat())
            {
                enemy.Flip(enemy.direction);
                rb.velocity = new Vector2(enemy.enemyData.AttackVelocityRetreat.x * -enemy.direction, enemy.enemyData.AttackVelocityRetreat.y);
                return;
            }
            enemy.SetVelocity(new Vector2(0, rb.velocity.y));
            animator.SetFloat("AttackMultplier", enemy.attackSpeed);
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

        public override void Exit() => base.Exit();
    }
}
