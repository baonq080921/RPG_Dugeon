using stateMachine;

namespace enemy
{
    /// <summary>Stun/hit state specific to <see cref="EnemySlime"/>.</summary>
    public class EnemySlimeStunState : EnemyState
    {
        public EnemySlimeStunState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            if (enemy.isDead)
                enemy.ChangeToDiedState();

            stateTimer = enemy.entityStat.StunDuration;
            enemy.CanCounter = false;
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer > 0f) return;

            if (enemy.IsPlayerDetected())
                stateMachine.ChangeState(enemy.enemyChaseState);
            else
                stateMachine.ChangeState(enemy.enemyIdleState);
        }
    }
}
