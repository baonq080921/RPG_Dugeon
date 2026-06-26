using UnityEngine;

namespace stateMachine
{
    /// <summary>
    /// Base animated state for the state machine. Holds the owning <see cref="StateMachine"/>,
    /// the animator bool that gates the state's animation, and a per-state timer.
    /// Lives in the stateMachine package (not Entity) because it depends only on the state machine
    /// and Unity components — keeping the stateMachine package free of any dependency on game entities.
    /// </summary>
    public class EntityState
    {
        protected StateMachine stateMachine;
        protected string animBoolName;
        protected Animator animator;
        protected Rigidbody2D rb;
        protected bool isTriggered;
        protected float stateTimer;

        public EntityState(StateMachine stateMachine, string animBoolName)
        {
            this.stateMachine = stateMachine;
            this.animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            animator.SetBool(animBoolName, true);
            isTriggered = false;
        }


        public virtual void Update()
        {
            stateTimer -= Time.deltaTime;
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
