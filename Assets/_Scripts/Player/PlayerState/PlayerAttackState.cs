using UnityEngine;
using stateMachine;

namespace player
{
    /// <summary>
    /// Ground combo attack state. Combo limit and velocities come from
    /// <see cref="CharacterData"/> via the player, so subclasses only need to
    /// override this when the attack *behavior* changes, not just the numbers.
    /// </summary>
    public class PlayerAttackState : PlayerState
    {
        private const int AttackDefaultIndex = 1;
        private Vector2 _attackVelocity;
        private int _attackDirection;
        private int _attackIndex = AttackDefaultIndex;
        private float _lastAttackTime;
        private bool _attackComboQueue;

        /// <inheritdoc/>
        public PlayerAttackState(Player player, StateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName) {}

        /// <inheritdoc/>
        public override void Enter()
        {
            base.Enter();
            _attackComboQueue = false;
            _attackDirection = player.movementInput.x != 0
                ? (int)Mathf.Sign(player.movementInput.x)
                : (int)player.direction;
            ResetComboAttack();
            ApplyAttackVelocity();
            animator.SetInteger("AttackIndex", _attackIndex);
        }

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();
            if (!isTriggered) return;

            if (input.Player.Dash.WasPressedThisFrame())
                stateMachine.ChangeState(player.playerDashState);
            if (input.Player.Jump.WasPressedThisFrame())
                stateMachine.ChangeState(player.playerJumpState);

            if (input.Player.BasicAttack.WasPressedThisFrame())
                _attackComboQueue = true;

            HandleStateExit();
        }

        /// <summary>
        /// Decides what happens after the current attack animation finishes.
        /// Override in a character subclass to add special moves (e.g. a finisher on the last hit).
        /// </summary>
        protected virtual void HandleStateExit()
        {
            if (_attackIndex >= player.ComboLimit)
            {
                player.StartAttackCooldown();
                stateMachine.ChangeState(player.playerIdleState);
            }
            else if (_attackComboQueue)
            {
                animator.SetBool(animBoolName, false);
                player.EnterAttackComboCoroutine();
            }
            else
            {
                stateMachine.ChangeState(player.playerIdleState);
            }
        }

        /// <inheritdoc/>
        public override void Exit()
        {
            base.Exit();
            _attackIndex++;
            _lastAttackTime = Time.time;
        }

        private void ResetComboAttack()
        {
            if (Time.time > _lastAttackTime + player.timeResetCombo)
                _attackIndex = AttackDefaultIndex;
        }

        private void ApplyAttackVelocity()
        {
            if (_attackIndex > player.ComboLimit)
                _attackIndex = AttackDefaultIndex;
            _attackVelocity = player.attackVelocity[_attackIndex - 1];
            player.SetVelocity(new Vector2(_attackDirection * _attackVelocity.x, _attackVelocity.y));
        }
    }
}
