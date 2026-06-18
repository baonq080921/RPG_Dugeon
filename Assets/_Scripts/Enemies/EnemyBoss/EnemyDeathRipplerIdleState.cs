using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>Idle state specific to <see cref="EnemyDeathRippler"/>.</summary>
    public class EnemyDeathRipplerIdleState : EnemyGroundedState
    {
        public EnemyDeathRipplerIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            enemy.SetVelocity(new Vector2(0, rb.velocity.y));
        }

        public override void Exit() => base.Exit();
    }
}
