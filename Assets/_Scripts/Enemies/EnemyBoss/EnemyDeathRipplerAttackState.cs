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
            if (enemy.ShouldEnemyRetreat())
            {
                if(_enemyDeathRippler.ShouldTelePort()) // teleport to the back of the player
                {
                    stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerTeleportBackState);
                }
                enemy.FacePlayer();
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
            if(_enemyDeathRippler.entityHealth.CurrentHealth/_enemyDeathRippler.entityStat.GetHealthValue() < 0.3f 
                && !_enemyDeathRippler.isLastAttackAttempt)
            {
                stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerSpecialLastState);
                return;
            }
            if (!isTriggered) return;

            if (enemy.IsPlayerDetected())
                stateMachine.ChangeState(_enemyDeathRippler.enemyChaseState);
            else
                stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerBattleState);
        }

        public override void Exit() => base.Exit();
    }
}
