using stateMachine;

namespace enemy
{
    /// <summary>Stun/hit state specific to <see cref="EnemySkeleton"/>.</summary>
    public class EnemySkeletonStunState : EnemyState
    {
        public EnemySkeletonStunState(Enemy enemy, StateMachine stateMachine, string animBoolName)
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
