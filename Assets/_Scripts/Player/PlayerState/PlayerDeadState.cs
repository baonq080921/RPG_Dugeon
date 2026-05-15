using stateMachine;
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
            rb.simulated = false;
            input.Disable();
        }
        // public override void Update()
        // {
        //     base.Update();
        //     if(player)
        // }
    }
}
