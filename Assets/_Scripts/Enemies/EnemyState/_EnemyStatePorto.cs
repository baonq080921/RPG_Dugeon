using enemy;
using stateMachine;

namespace enemy
{
    public class EnemyStateProto : EnemyState
    {
        public EnemyStateProto(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
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
}