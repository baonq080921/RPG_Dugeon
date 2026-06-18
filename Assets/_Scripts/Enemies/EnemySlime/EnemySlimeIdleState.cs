using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>Idle state specific to <see cref="EnemySlime"/>.</summary>
    public class EnemySlimeIdleState : EnemyState
    {
        public EnemySlimeIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            stateTimer = enemy.enemyData.IdleTime;
            enemy.SetVelocity(new Vector2(0, rb.velocity.y));
        }

        public override void Update()
        {
            base.Update();

            if (enemy.IsPlayerDetected())
            {
                stateMachine.ChangeState(enemy.enemyChaseState);
                return;
            }

            if (stateTimer <= 0)
                stateMachine.ChangeState(enemy.enemyMoveState);
        }

        public override void Exit() => base.Exit();
    }
}
