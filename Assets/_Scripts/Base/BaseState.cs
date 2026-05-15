using stateMachine;
public class BaseState : EntityState
{
    public BaseState(StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}