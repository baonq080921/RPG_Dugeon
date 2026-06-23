using stateMachine;

namespace enemy
{
    /// <summary>Stun/hit state specific to <see cref="EnemyDeathRippler"/>.</summary>
    public class EnemyArcherStunState : EnemyState
    {
        private EnemyArcher _enemyArcher;
        public EnemyArcherStunState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyArcher = enemy as EnemyArcher;
        }

        public override void Enter()
        {
            base.Enter();
            if (_enemyArcher.isDead)
                _enemyArcher.ChangeToDiedState();

            stateTimer = _enemyArcher.entityStat.StunDuration;
        }

        public override void Update()
        {
            base.Update();
            if (stateTimer > 0f) return;

            if (_enemyArcher.IsPlayerDetected())
                stateMachine.ChangeState(_enemyArcher.enemyMoveState);
            else
                stateMachine.ChangeState(_enemyArcher.enemyIdleState);
        }
    }
}
