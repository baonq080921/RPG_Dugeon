using stateMachine;
using UnityEngine;

namespace enemy
{
    /// <summary>
    /// Moves toward the detected player at chase speed.
    /// Jumps when a wall or step blocks the path while grounded.
    /// Transitions to Attack when close enough, or back to Idle if the player escapes detection.
    /// </summary>
    public class EnemyChaseState : EnemyState
    {
        private float _jumpCooldownTimer;
        private const float JumpCooldown = 0.5f;

        public EnemyChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName) {}

        public override void Enter()
        {
            base.Enter();
            _jumpCooldownTimer = 0f;
        }

        public override void Update()
        {
            base.Update();
            _jumpCooldownTimer -= Time.deltaTime;

            if (!enemy.IsPlayerDetected())
            {
                stateMachine.ChangeState(enemy.enemyIdleState);
                return;
            }

            if (enemy.IsPlayerInAttackRange())
            {
                stateMachine.ChangeState(enemy.enemyAttackState);
                return;
            }

            FacePlayer();

            if (enemy.isTouchingWall && enemy.isGrounded && _jumpCooldownTimer <= 0f)
            {
                enemy.Jump();
                _jumpCooldownTimer = JumpCooldown;
            }

            enemy.SetVelocity(new Vector2(enemy.direction * enemy.enemyData.MoveSpeed * enemy.enemyData.MoveMultiplier, rb.velocity.y));
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void FacePlayer()
        {
            if (enemy.DetectedPlayer == null) return;
            float newDir = enemy.DetectedPlayer.position.x > enemy.transform.position.x ? 1f : -1f;
            if (newDir == enemy.direction) return;
            enemy.SetDirection(newDir);
            enemy.Flip(newDir);
        }
    }
}
