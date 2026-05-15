using stateMachine;

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

        public override void Update()
        {
            base.Update();
            if (player.IsKnocked) return;

            if (player.isGrounded)
                stateMachine.ChangeState(player.playerIdleState);
            else
                stateMachine.ChangeState(player.playerFallState);
        }
    }
}
