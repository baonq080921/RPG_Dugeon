using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Briefly freezes the enemy after taking a hit.
    /// Exits to Chase or Idle once <see cref="EnemyData.StunDuration"/> elapses.
    /// </summary>
    public class EnemyStunState : EnemyState
    {
        private EnemyVfx _enemyVfx;
        public EnemyStunState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            if(enemy.isDead)
                stateMachine.ChangeState(enemy.enemyDeathState);

                
            stateTimer = enemy.enemyData.StunDuration;
            enemy.ApplyKnockBack(enemy.enemyData.Damage);
            enemy.CanCounter = false;
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer > 0f) return;

            if (enemy.IsPlayerDetected())
                stateMachine.ChangeState(enemy.enemyChaseState);
            else
                stateMachine.ChangeState(enemy.enemyIdleState);
        }
    }
}
