using UnityEngine;
using stateMachine;
using Base;
namespace player
{
    public class PlayerAttackState : EntityState
    {
        private int _attackDefaultIndex = 1;
        private Vector2 _attackVelocity;
        private int _attackDirection;
        private int _attackIndex = 1;
        private float _lastAttackTime;
        private int _comboLimit = 3;
        private bool _attackComboQueue;
        public PlayerAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _attackComboQueue = false;
            _attackDirection = player.movementInput.x !=0 ? _attackDirection = (int)player.movementInput.x :_attackDirection = (int)player.direction;
            ResetComboAttack();
            ApplyAttackVelocity();
            animator.SetInteger("AttackIndex", _attackIndex);
        }

        public override void Update()
        {
            base.Update();

            if(input.Player.Dash.WasPressedThisFrame())
                stateMachine.ChangeState(player.playerDashState);
            if(input.Player.Jump.WasPressedThisFrame())
                stateMachine.ChangeState(player.playerJumpState);


            if (input.Player.BasicAttack.WasPressedThisFrame())
                _attackComboQueue = true;
            if (isTriggered)
            {
                HandleStateExit();
            }
        }

        private void HandleStateExit()
        {
            if (_attackIndex >= _comboLimit) // when player hit the limit combo have some cooldown before attacking again
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
                stateMachine.ChangeState(player.playerIdleState);
        }

        public override void Exit()
        {
            base.Exit();
            _attackIndex++;
            _lastAttackTime = Time.time;
        }

        private void ResetComboAttack()
        {
            if (Time.time > _lastAttackTime + player.timeResetCombo)
                _attackIndex = _attackDefaultIndex;
        }

        private void ApplyAttackVelocity()
        {
            if (_attackIndex > _comboLimit)
                _attackIndex = _attackDefaultIndex;
            _attackVelocity = player.attackVelocity[_attackIndex - 1];
            player.SetVelocity(new Vector2(_attackDirection * _attackVelocity.x, _attackVelocity.y));
        }
    }
}