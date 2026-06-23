using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>Death state specific to <see cref="EnemyDeathRippler"/>.</summary>
    public class EnemyArcherDeathState : EnemyState
    {
        public EnemyArcherDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            enemy.col.enabled = false;
            enemy.SetVelocity(Vector2.zero);
            stateTimer = 2f;
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer <= 0)
                enemy.ReturnToPool();
        }

        public override void Exit() => base.Exit();
    }
}
