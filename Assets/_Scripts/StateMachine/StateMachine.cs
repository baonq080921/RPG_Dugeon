using Base;
namespace stateMachine
{
    public class StateMachine
    {
        
        public EntityState currentState { get; private set; }
        public EntityState previousState { get; private set; }

        public void Initialize(EntityState startingState)
        {
            currentState = startingState;
            currentState.Enter();
        }

        public void ChangeState(EntityState newState)
        {
            previousState = currentState;
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }
    }

}
