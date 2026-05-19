using stateMachine;
using UnityEngine;
namespace player
{
    /// <summary>
    /// Entered when the player receives a knockback hit.
    /// Blocks all input until <see cref="Entity.IsKnocked"/> clears,
    /// then falls through to Idle or Fall.
    /// </summary>
    public class PlayerKnockBackState : PlayerState
    {
        public PlayerKnockBackState(Player player, StateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) { }


        public override void Enter()
        {
            base.Enter();
            stateTimer = player.entityStat.StunDuration;
            input.Disable();
            player.SetVelocity(Vector2.zero);
        }
        public override void Update()
        {
            base.Update();
            if(stateTimer > 0) return;

            if (player.IsKnocked) return;

            if (player.isGrounded)
                stateMachine.ChangeState(player.playerIdleState);
            else
                stateMachine.ChangeState(player.playerFallState);
        }

        public override void Exit()
        {
            base.Exit();
            input.Enable();
        }
    }
}
