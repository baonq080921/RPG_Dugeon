using enemy;

namespace enemy
{
    /// <inheritdoc/>
    public class EnemySkeleton : Enemy
    {
        public EnemySkeletonDeathState enemyDeathState { get; private set; }

        protected override void InitializeStates()
        {
            enemyIdleState = new EnemySkeletonIdleState(this, stateMachine, "Idle");
            enemyMoveState = new EnemySkeletonMoveState(this, stateMachine, "Move");
            enemyChaseState = new EnemySkeletonChaseState(this, stateMachine, "Move");
            enemyAttackState = new EnemySkeletonAttackState(this, stateMachine, "Attack");
            enemyStunState = new EnemySkeletonStunState(this, stateMachine, "Hit");
            enemyDeathState = new EnemySkeletonDeathState(this, stateMachine, "Died");
        }

        public override void ChangeToDiedState()
        {
            base.ChangeToDiedState();
            stateMachine.ChangeState(enemyDeathState);
        }
    }
}
