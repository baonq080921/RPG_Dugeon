using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>Base state for all enemy states.</summary>
    public abstract class EnemyState : EntityState
    {
        protected Enemy enemy;
        protected EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
        {
            this.enemy = enemy;
            animator = enemy.animator;
            rb = enemy.rb;
        }

        public override void Update()
        {
            base.Update();
            animator.SetFloat("MoveMultiplier", enemy.enemyData.MoveMultiplier);
        }
    }
}
