
using stateMachine;
using Base;
using player;
using UnityEngine;
namespace Base
{
    public abstract class EntityState
    {
        protected StateMachine stateMachine;
        protected string animBoolName;
        protected Player player;
        protected Animator animator;
        protected bool isTriggered;
        protected PlayerInputSet input;
        protected Rigidbody2D rb;
        public EntityState(Player player,StateMachine stateMachine, string animBoolName)
        {
                this.stateMachine = stateMachine;
                this.animBoolName = animBoolName;
                this.player = player;
                animator = player.animator;
                rb = player.rb;
                input = player.input;
        }

        public virtual void Enter()
        {
            animator.SetBool(animBoolName, true);
            isTriggered = false;
        }
        public virtual void Update()
        {
            // DebugCustom.Log($"Updating state: {animBoolName}");
            animator.SetFloat("yVelocity",rb.velocity.y);
        }
        public virtual void Exit()
        {
            animator.SetBool(animBoolName, false);
        }



        public void TriggerAnimation()
        {
            if (isTriggered) return;
            isTriggered = true;
        }
    }

}