using stateMachine;
using UnityEngine;
namespace player
{
    public class PlayerDeadState : PlayerState
    {
        public PlayerDeadState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered Dead State");
            rb.isKinematic = false;
            rb.velocity = new Vector2(0.001f,0.001f);
            input.Disable();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();    
            rb.simulated = true;
            input.Enable();
        }
    }
}
