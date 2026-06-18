using enemy;
using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Attack state specific to <see cref="EnemyDeathRippler"/>.
    /// Locks the boss in place while the attack animation plays.
    /// </summary>
    public class EnemyDeathRipplerAttackState : EnemyState
    {
        private EnemyDeathRippler _enemyDeathRippler;
        public EnemyDeathRipplerAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyDeathRippler = enemy as EnemyDeathRippler;
        }

        public override void Enter()
        {
            base.Enter();
            float randomValue = Random.Range(0f,1f);
            if (enemy.ShouldEnemyRetreat())
            {
                if(_enemyDeathRippler.ShouldTelePort()) // teleport to the back of the player
                {
                    stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerTeleportBackState);
                }
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
