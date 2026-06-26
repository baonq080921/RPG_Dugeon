using stateMachine;
using UnityEngine;

namespace enemy
{
    public class EnemyGroundedState : EnemyState
    {
        private EnemyDeathRippler _enemyDeathRippler;
        public EnemyGroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
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
            if (enemy.IsPlayerDetected())
                _enemyDeathRippler.TriggerAggro();

            if (_enemyDeathRippler.HasAggro)
                stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerBattleState);
        }
    }
}
