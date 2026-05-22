using System;
using System.Collections;
using Base;
using stateMachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace player
{
    /// <summary>
    /// Base player entity. All per-character stats come from <see cref="CharacterData"/>.
    /// Override <see cref="CreateStates"/> in a subclass to swap individual states for a
    /// different character without touching any shared logic.
    /// </summary>
    [RequireComponent(typeof(SkillManager))]
    public class Player : Entity
    {
        [field: SerializeField] public CharacterData Data { get; private set; }
        /// <inheritdoc/>
        public override LayerMask LayerMask => Data.WhatisTarget;


        public float moveSpeed => Data.MoveSpeed;
        public float jumpForce => Data.JumpForce;
        public int MaxJumpCount => Data.MaxJumpCount;
        public float airControlFactor => Data.AirControlFactor;
        public float slideDownSpeed => Data.SlideDownSpeed;
        public Vector2 WallJumpForce => Data.WallJumpForce;
        public float dashSpeed => Data.DashSpeed;
        public float dashDuration => Data.DashDuration;
        public float dashCooldown => Data.DashCooldown;
        public Vector2[] attackVelocity => Data.AttackVelocities;
        public int ComboLimit => Data.ComboLimit;
        public float timeResetCombo => Data.TimeResetCombo;

        /// <summary>Fired when this player becomes the active controlled character.</summary>
        public static event Action<Player> ActivePlayerChanged;

        /// <summary>Fired when dash cooldown begins. Parameter is the total cooldown duration.</summary>
        public event Action<float> DashCooldownStarted;

        public bool canDash { get; private set; } = true;
        private float _dashCooldownTimer;

        public bool canAttack { get; private set; } = true;
        private float _attackCooldownTimer;

        public bool canAirAttack { get; private set; } = true;
        private float _airAttackCooldownTimer;

        // Protected set so character subclasses can swap states in CreateStates()
        public PlayerIdleState playerIdleState { get; protected set; }
        public PlayerMoveState playerMovementState { get; protected set; }
        public PlayerJumpState playerJumpState { get; protected set; }
        public PlayerFallState playerFallState { get; protected set; }
        public PlayerWallSildeState playerWallSlideState { get; protected set; }
        public PlayerWallJumpState playerWallJumpState { get; protected set; }
        public PlayerDashState playerDashState { get; protected set; }
        public PlayerAttackState playerBasicAttackState { get; protected set; }
        public PlayerJumpAttackState playerJumpAttackState { get; protected set; }
        public PlayerKnockBackState playerKnockBackState { get; protected set; }
        public PlayerDeadState playerDeadState {get; private set;}
        public PlayerCounterState playerCounterState {get; private set;}

        public PlayerInputSet input;
        public SkillManager SkillManager { get; private set; }
        public AfterImageEffect AfterImageEffect { get; private set; }

        public Vector2 movementInput { get; private set; }
        public bool isJump { get; private set; }
        public bool JumpJustPressed { get; private set; }
        public bool DashJustPressed { get; private set; }
        public int JumpCount { get; set; }
        public float LastWallJumpDirection { get; set; } = 0f;
        private Coroutine _queueComboCouroutine;


        [field:SerializeField]public float attackElapsedTime { get; set; }

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            AfterImageEffect = GetComponent<AfterImageEffect>();
            SkillManager = GetComponent<SkillManager>();
            input = new PlayerInputSet();
            CreateStates();
        }

        /// <summary>
        /// Instantiates all player states. Override in a character subclass to replace
        /// specific states — call base.CreateStates() first, then reassign only what differs.
        /// </summary>
        protected virtual void CreateStates()
        {
            playerIdleState = new PlayerIdleState(this, stateMachine, "Idle");
            playerMovementState = new PlayerMoveState(this, stateMachine, "Move");
            playerJumpState = new PlayerJumpState(this, stateMachine, "JumpFall");
            playerFallState = new PlayerFallState(this, stateMachine, "JumpFall");
            playerWallSlideState = new PlayerWallSildeState(this, stateMachine, "WallSlide");
            playerWallJumpState = new PlayerWallJumpState(this, stateMachine, "JumpFall");
            playerDashState = new PlayerDashState(this, stateMachine, "Dash");
            playerBasicAttackState = new PlayerAttackState(this, stateMachine, "BasicAttack");
            playerJumpAttackState = new PlayerJumpAttackState(this, stateMachine, "BasicAttack");
            playerKnockBackState = new PlayerKnockBackState(this, stateMachine, "Hit");
            playerDeadState = new PlayerDeadState(this, stateMachine,"Dead");
            playerCounterState = new PlayerCounterState(this, stateMachine, "EnterCounter");
            SkillManager.RegisterState((int)SkillName.CounterSkill, playerCounterState);   
            }

        void OnEnable()
        {
            ActivePlayerChanged?.Invoke(this);
            input.Enable();
            input.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            input.Player.Movement.canceled += ctx => movementInput = Vector2.zero;
            input.Player.Jump.performed += ctx => { isJump = true; JumpJustPressed = true; };
            input.Player.Jump.canceled += ctx => isJump = false;
            input.Player.Dash.performed += ctx => { DashJustPressed = true; };

            // Keyboard fallback for skill slot 0 (Counter). Mobile uses on-screen SkillButton instead.
            input.Player.Counter.performed += ctx => SkillManager.PressSkill(0);
        }

        void OnDisable()
        {
            input.Disable();
        }

        protected override void Start()
        {
            base.Start();
            stateMachine.Initialize(playerIdleState);
        }

        protected override void Update()
        {
            base.Update();
            TickDashCooldown();
            TickAttackCooldown();
            TickAirAttackCooldown();
        }

        public override void ApplyKnockBack(float damage)
        {
            if(damage/entityStat.GetHealthValue() < 0.5f) return;
            base.ApplyKnockBack(damage);
        }

        public void EnterAttackComboCoroutine()
        {
            if (_queueComboCouroutine != null)
                StopCoroutine(_queueComboCouroutine);
            _queueComboCouroutine = StartCoroutine(AttackQueueCouroutine());
        }
        IEnumerator AttackQueueCouroutine()
        {
            yield return new WaitForEndOfFrame();
            stateMachine.ChangeState(playerBasicAttackState);
        }

        public void StartDashCooldown()
        {
            canDash = false;
            _dashCooldownTimer = dashCooldown;
            DashCooldownStarted?.Invoke(dashCooldown);
        }

        public override void Die()
        {
            base.Die();
            EventBus<PlayerDiedEvent>.Raise(new PlayerDiedEvent());
            stateMachine.ChangeState(playerDeadState);
        }

        private void TickDashCooldown()
        {
            if (_dashCooldownTimer <= 0) return;
            _dashCooldownTimer -= Time.deltaTime;
            if (_dashCooldownTimer <= 0)
                canDash = true;
        }

        public void StartAttackCooldown()
        {
            canAttack = false;
            _attackCooldownTimer = Data.ComboEndDelay;
        }

        public void StartAirAttackCooldown()
        {
            canAirAttack = false;
            _airAttackCooldownTimer = Data.AirAttackCooldown;
        }

        private void TickAttackCooldown()
        {
            if (_attackCooldownTimer <= 0) return;
            _attackCooldownTimer -= Time.deltaTime;
            if (_attackCooldownTimer <= 0)
                canAttack = true;
        }

        private void TickAirAttackCooldown()
        {
            if (_airAttackCooldownTimer <= 0) return;
            _airAttackCooldownTimer -= Time.deltaTime;
            if (_airAttackCooldownTimer <= 0)
                canAirAttack = true;
        }

        /// <summary>Clears JumpJustPressed after it has been consumed by a state.</summary>
        public void ConsumeJump() => JumpJustPressed = false;

        public void ConsumeDash() => DashJustPressed = false;

        public void SetCanDash(bool value) => canDash = value;
        public override void SetVelocity(Vector2 velocity)
        {
            base.SetVelocity(velocity);
            HandleFlip(movementInput.x);
        }


        public void HandleFlip(float horizontalInput)
        {
            if (horizontalInput > 0f)
            {
                direction = 1f;
                Flip(direction);
            }
            else if (horizontalInput < 0f)
            {
                direction = -1f;
                Flip(direction);
            }
        }
    }
}
