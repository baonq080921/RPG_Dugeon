using enemy;
using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Attack state specific to <see cref="EnemyDeathRippler"/>.
    /// Locks the boss in place while the attack animation plays.
    /// </summary>
    public class EnemyDeathRipplerBattleState : EnemyState
    {
        private EnemyDeathRippler _enemyDeathRippler;
        public EnemyDeathRipplerBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyDeathRippler = enemy as EnemyDeathRippler;
        }

        public override void Enter()
        {
            base.Enter();
           
        }

        public override void Update()
        {
            base.Update();
            enemy.FacePlayer();
            if(enemy.IsInAttackRange() && enemy.IsPlayerDetected())
            {
                stateMachine.ChangeState(_enemyDeathRippler.enemyAttackState);
            }
            else
            {
                stateMachine.ChangeState(_enemyDeathRippler.enemyChaseState);
            }
        }

        public override void Exit() => base.Exit();
    }
}
