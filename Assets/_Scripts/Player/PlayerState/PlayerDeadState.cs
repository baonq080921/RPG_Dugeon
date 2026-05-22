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
            rb.simulated = false;
            input.Disable();
        }

        public override void Update()
        {
            base.Update();
            if(Input.GetKeyDown(KeyCode.R))
            {
                // For testing purposes, respawn the player when R is pressed
                stateMachine.ChangeState(player.playerIdleState);
            }
        }

        public override void Exit()
        {
            base.Exit();    
            rb.simulated = true;
            input.Enable();
        }
    }
}
