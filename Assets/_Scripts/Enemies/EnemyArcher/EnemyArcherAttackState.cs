using UnityEngine;
using stateMachine;

namespace enemy
{
    /// <summary>
    /// Locks the archer in place while the attack animation plays, then returns to <see cref="EnemyArcherCombatState"/>.
    /// </summary>
    public class EnemyArcherAttackState : EnemyState
    {
        private EnemyArcher _enemyArcher;

        public EnemyArcherAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyArcher = enemy as EnemyArcher;
        }

        public override void Enter()
        {
            base.Enter();
            enemy.SetVelocity(Vector2.zero);
            animator.SetFloat("AttackMultplier", enemy.attackSpeed);
        }

        public override void Update()
        {
            base.Update();
            _enemyArcher.FacePlayer();
            if (!_enemyArcher.IsInAttackRange())
            {
                stateMachine.ChangeState(_enemyArcher.enemyMoveState);
                return;
            }
            if (isTriggered)
            {
                stateMachine.ChangeState(_enemyArcher.enemyArcherMoveBackState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            _enemyArcher.StartAttackCooldown();
        }
    }
}
