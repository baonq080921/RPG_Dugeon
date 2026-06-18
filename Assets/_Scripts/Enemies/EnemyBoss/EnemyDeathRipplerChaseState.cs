using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Chase state specific to <see cref="EnemySlime"/>.
    /// Moves toward the player and jumps over obstacles. Transitions to attack when in range.
    /// </summary>
    public class EnemyDeathRipplerChaseState : EnemyState
    {
        private EnemyDeathRippler _enemyDeathRippler;
        public EnemyDeathRipplerChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName)
        {
            _enemyDeathRippler = enemy as EnemyDeathRippler;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Enter chase boss state");
        }

        public override void Update()
        {
            base.Update();
            if (_enemyDeathRippler.IsInAttackRange())
            {
                stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerBattleState);
                return;
            }

            enemy.FacePlayer();
            float chaseSpeed = _enemyDeathRippler.enemyData.MoveSpeed * enemy.enemyData.MoveMultiplier;
            if (!_enemyDeathRippler.isGrounded || _enemyDeathRippler.isTouchingWall)
            {
                chaseSpeed = 0.001f;
                enemy.SetVelocity(new Vector2(enemy.direction * chaseSpeed , rb.velocity.y));
                stateMachine.ChangeState(_enemyDeathRippler.enemyDeathRipplerTeleportState);
            }
            else
                enemy.SetVelocity(new Vector2(enemy.direction * chaseSpeed , rb.velocity.y));
        }

        public override void Exit() => base.Exit();
    }
}
