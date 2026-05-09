
using stateMachine;
using UnityEngine;

namespace enemy
{
    public abstract class EnemyState : EntityState
    {
        protected Enemy enemy;
        protected EnemyState(Enemy enemy,StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
        {
            this.enemy = enemy;
            animator = enemy.animator;
            rb = enemy.rb;
        }
    }
}