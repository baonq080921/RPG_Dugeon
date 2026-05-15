using UnityEngine;
using stateMachine;

namespace enemy
{
    public class EnemyDeathState : EnemyState
    {
        public EnemyDeathState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            enemy.SetVelocity(new Vector2(3f,10f));
            stateTimer = 2f;
            enemy.col.enabled = false;
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer <= 0)
                enemy.ReturnToPool();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}