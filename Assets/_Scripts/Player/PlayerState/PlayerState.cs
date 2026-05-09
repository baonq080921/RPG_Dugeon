
using stateMachine;
using player;
using UnityEngine;
namespace player
{
    public abstract class PlayerState : EntityState
    {
        
        protected Player player;
        protected PlayerInputSet input;
        public PlayerState(Player player,StateMachine stateMachine, string animBoolName):base(stateMachine,animBoolName)
        {       
                this.player = player;
                animator = player.animator;
                rb = player.rb;
                input = player.input;
        }



        public override void Enter()
        {
            base.Enter();
            
        }

        public override void Update()
        {
            base.Update();
            animator.SetFloat("yVelocity",rb.velocity.y);

        }

        public override void Exit()
        {
            base.Exit();
        }



        public void TriggerAnimation()
        {
            if (isTriggered) return;
            isTriggered = true;
        }
    }

}