using UnityEngine;
using stateMachine;

namespace player
{
    /// <summary>
    /// Handles the jump attack state.
    /// JumpAttack_1: triggered by pressing Jump + Attack from the ground.
    /// JumpAttack_2: triggered by pressing Attack while holding up direction in the air.
    /// </summary>
    public class PlayerJumpAttackState : PlayerState
    {
        /// <summary>When true, applies jump force on enter (triggered from ground).</summary>
        public bool IsFromGround { get; set; }

        /// <summary>When true, plays the JumpAttack_2 animation (attack + up in air).</summary>
        public bool IsUpAttack { get; set; }

        /// <inheritdoc/>
        public PlayerJumpAttackState(Player player, StateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) {}

        /// <inheritdoc/>
        public override void Enter()
        {
            base.Enter();
            if (IsFromGround)
            {
                player.JumpCount++;
                player.SetVelocity(new Vector2(rb.velocity.x, player.jumpForce));
            }
            animator.SetBool(IsUpAttack ? "JumpAttack_2" : "JumpAttack", true);
        }

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();
            if (!player.isGrounded) return;
            stateMachine.ChangeState(player.playerIdleState);
        }

        /// <inheritdoc/>
        public override void Exit()
        {
            base.Exit();
            animator.SetBool("JumpAttack", false);
            animator.SetBool("JumpAttack_2", false);
        }


        public void Trigger()
        {
            stateMachine.ChangeState(player.playerFallState);
        }
    }
}