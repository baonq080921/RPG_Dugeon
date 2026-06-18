using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Chase state specific to <see cref="EnemySlime"/>.
    /// Moves toward the player and jumps over obstacles. Transitions to attack when in range.
    /// </summary>
    public class EnemySlimeChaseState : EnemyState
    {
        private float _jumpCooldownTimer;
        private const float JumpCooldown = 0.5f;

        public EnemySlimeChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName)
            : base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();
            _jumpCooldownTimer = 0f;
        }

        public override void Update()
        {
            base.Update();
            _jumpCooldownTimer -= Time.deltaTime;
            if (enemy.IsInAttackRange())
            {
                stateMachine.ChangeState(enemy.enemyAttackState);
                return;
            }

            enemy.FacePlayer();

            if (enemy.isTouchingWall && enemy.isGrounded && _jumpCooldownTimer <= 0f)
            {
                enemy.Jump();
                _jumpCooldownTimer = JumpCooldown;
            }

            enemy.SetVelocity(new Vector2(enemy.direction * enemy.enemyData.MoveSpeed * enemy.enemyData.MoveMultiplier, rb.velocity.y));
        }

        public override void Exit() => base.Exit();
    }
}
