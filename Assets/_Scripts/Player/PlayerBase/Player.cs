using System;
using System.Collections;
using Base;
using Interfaces;
using UnityEngine;

namespace player
{
    /// <summary>
    /// Base player entity. All per-character stats come from <see cref="CharacterData"/>.
    /// Override <see cref="CreateStates"/> in a subclass to swap individual states for a
    /// different character without touching any shared logic.
    /// </summary>
    public class Player : Entity
    {
        [field: SerializeField] public CharacterData Data { get; private set; }
        /// <inheritdoc/>
        public override LayerMask LayerMask => Data.WhatisTarget;

        public float moveSpeed { get; private set; }
        public float jumpForce { get; private set; }
        public int MaxJumpCount { get; private set; }
        public float airControlFactor { get; private set; }
        public float slideDownSpeed { get; private set; }
        public Vector2 WallJumpForce { get; private set; }
        public float dashSpeed { get; private set; }
        public float dashDuration { get; private set; }
        public float dashCooldown { get; private set; }
        public Vector2[] attackVelocity { get; private set; }
        public int ComboLimit { get; private set; }
        public float timeResetCombo { get; private set; }


        /// <summary>The currently active controlled player. Null when no player is active.</summary>
        public static Player ActivePlayer { get; private set; }

        /// <summary>Fired when this player becomes the active controlled character.</summary>
        public static event Action<Player> ActivePlayerChanged;
        public bool canDash { get; private set; } = true;
        private float _dashCooldownTimer;

        public bool canAttack { get; private set; } = true;
        private float _attackCooldownTimer;

        public bool canAirAttack { get; private set; } = true;
        private float _airAttackCooldownTimer;
        private float _boostingCoolDownTimer;

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
        public PlayerDismantleState playerDismantleState {get; private set;}
        public PlayerDomainExpasionState playerDomainExpasionState {get; private set;}
        public PlayerInputSet input;
        public SkillButtonHandler SkillButtonHandler { get; private set; }
        public AfterImageEffect AfterImageEffect { get; private set; }
        private EventBinding<GamePauseChangedEvent> _pauseBinding;
        private EventBinding<PlayerBoostingAmountEvent> _eventBoostingBinding;


        public Vector2 movementInput { get; private set; }
        public bool isJump { get; private set; }
        public bool DashJustPressed { get; private set; }
        public int JumpCount { get; set; }
        public float LastWallJumpDirection { get; set; } = 0f;
        private Coroutine _queueComboCouroutine;
        private Coroutine _boostingCoroutine;

        [field:SerializeField]public float attackElapsedTime { get; set; }

        //PLayer References
        public PlayerHealth playerHealth {get; private set;}
        public PlayerCombat playerCombat {get; private set;}
        public PlayerVfx playerVfx {get; private set;}
        public PlayerLevel playerLevel {get ; private set;}
        public PlayerInventory playerInventory {get; private set;}

        protected override void Awake()
        {
            base.Awake();
            AfterImageEffect = GetComponent<AfterImageEffect>();
            SkillButtonHandler = GetComponent<SkillButtonHandler>();
            playerHealth = GetComponent<PlayerHealth>();
            playerCombat = GetComponent<PlayerCombat>();
            playerVfx = GetComponent<PlayerVfx>();
            playerLevel = GetComponent<PlayerLevel>();
            playerInventory = GetComponent<PlayerInventory>();
            input = new PlayerInputSet();
            CreateStates();
            ServiceLocator.Register<Player>(this);
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
            playerDismantleState = new PlayerDismantleState(this,stateMachine,"CanDismantle");
            playerCounterState = new PlayerCounterState(this, stateMachine, "EnterCounter");
            playerDomainExpasionState = new PlayerDomainExpasionState(this, stateMachine,"CanDomain");

            //Skill Button Handler for moble
            SkillButtonHandler.RegisterState((int)ButtonSkillName.CounterSkill, playerCounterState); 
            SkillButtonHandler.RegisterState((int)ButtonSkillName.Dash, playerDashState);  
            SkillButtonHandler.RegisterState((int)ButtonSkillName.Dismantle,playerDismantleState);
            SkillButtonHandler.RegisterState((int)ButtonSkillName.Domain,playerDomainExpasionState);
            }

       protected override void OnEnable()
        {
            base.OnEnable();
            ActivePlayer = this;
            ActivePlayerChanged?.Invoke(this);
            input.Enable();
            input.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            input.Player.Movement.canceled += ctx => movementInput = Vector2.zero;            
            // Keyboard fallback for skill slot 0 (Counter). Mobile uses on-screen SkillButton instead.
            input.Player.Dash.performed += ctx => SkillButtonHandler.PressSkill(ButtonSkillName.Dash);
            input.Player.Counter.performed += ctx => SkillButtonHandler.PressSkill(ButtonSkillName.CounterSkill);
            input.Player.TimeEcho.performed += ctx => SkillButtonHandler.PressSkill(ButtonSkillName.TimeEcho);
            input.Player.Dismantle.performed += ctx => SkillButtonHandler.PressSkill(ButtonSkillName.Dismantle);
            input.Player.DomainExpasion.performed += ctx => SkillButtonHandler.PressSkill(ButtonSkillName.Domain);


            _pauseBinding = new EventBinding<GamePauseChangedEvent>(OnPauseChanged);
            EventBus<GamePauseChangedEvent>.Register(_pauseBinding);
            _eventBoostingBinding = new EventBinding<PlayerBoostingAmountEvent>(BoostingPlayerPhysicality);
            EventBus<PlayerBoostingAmountEvent>.Register(_eventBoostingBinding);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (ActivePlayer == this) ActivePlayer = null;
            input.Disable();
            EventBus<GamePauseChangedEvent>.Deregister(_pauseBinding);
            EventBus<PlayerBoostingAmountEvent>.Deregister(_eventBoostingBinding);
        }

       
        protected override void Start()
        {
            base.Start();
            LoadStatsFromData();
            stateMachine.Initialize(playerIdleState);
            RegisterSkillReferences();
        }

        private void LoadStatsFromData()
        {
            moveSpeed = Data.MoveSpeed;
            jumpForce = Data.JumpForce;
            MaxJumpCount = Data.MaxJumpCount;
            airControlFactor = Data.AirControlFactor;
            slideDownSpeed = Data.SlideDownSpeed;
            WallJumpForce = Data.WallJumpForce;
            dashSpeed = Data.DashSpeed;
            dashDuration = Data.DashDuration;
            dashCooldown = Data.DashCooldown;
            attackVelocity = Data.AttackVelocities;
            ComboLimit = Data.ComboLimit;
            timeResetCombo = Data.TimeResetCombo;
        }

        /// <summary>c
        /// RegisterSkill that references to the UISkillTree Node .Have to register to use the skill
        /// </summary> <summary>
        /// 
        /// </summary>
        private void RegisterSkillReferences()
        {
            var skillManager = ServiceLocator.Get<PlayerSkillManager>();
            if (skillManager == null) return;
            SkillButtonHandler.RegisterSkill((int)ButtonSkillName.Dash, skillManager.skillDash);
            SkillButtonHandler.RegisterSkill((int)ButtonSkillName.TimeEcho, skillManager.skillTimeEcho);
            SkillButtonHandler.RegisterSkill((int)ButtonSkillName.Dismantle, skillManager.skillDismantle);
            SkillButtonHandler.RegisterSkill((int)ButtonSkillName.Domain,skillManager.domainExpasionSkill);
        }

        private void OnPauseChanged(GamePauseChangedEvent e)
        {
            if (e.IsPause) input.Disable();
            else input.Enable();
        }


        private void BoostingPlayerPhysicality(PlayerBoostingAmountEvent e)
        {
            if(Data == null) return;
            if(_boostingCoroutine != null) StopCoroutine(_boostingCoroutine);
            _boostingCoroutine = StartCoroutine(BoostingCoroutine(e.Amount));
            
        }


        IEnumerator BoostingCoroutine(float amount)
        {
            moveSpeed = Data.MoveSpeed + amount;
            dashSpeed = Data.DashSpeed + amount;
            jumpForce = Data.JumpForce + amount;
            MaxJumpCount = Data.MaxJumpCount + (int)amount;
            yield return new WaitForSeconds(6f);
            moveSpeed = Data.MoveSpeed;
            dashSpeed = Data.DashSpeed;
            jumpForce = Data.JumpForce;
            MaxJumpCount = Data.MaxJumpCount;
        }

        protected override void Update()
        {
            if (ServiceLocator.Get<IGameState>()?.IsPause ?? false) return;
            base.Update();
            TickAttackCooldown();
            TickAirAttackCooldown();
            TickTimeEchoSkill();
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
        public override void Die()
        {
            base.Die();
            stateMachine.ChangeState(playerDeadState);
        }


        //Call this when the died animation play all through then open the gameover Menu
        public void RaiseDeadEvent()
        {
            EventBus<PlayerDiedEvent>.Raise(new PlayerDiedEvent());
        }

        public override void ApplyEffect(float scaleFactor, ElementType elementType)
        {
            if(elementType == ElementType.Ice)
            {
                moveSpeed = moveSpeed - moveSpeed * scaleFactor;
                dashSpeed = dashSpeed - dashSpeed * scaleFactor;
            }
        }


        public override void ResetEffect()
        {
            base.ResetEffect();
            moveSpeed = Data.MoveSpeed;
            dashSpeed = Data.DashSpeed;
        }

        public override void UnTargetableEnemy(bool canTarget)
        {
            base.UnTargetableEnemy(canTarget);
            if (canTarget)
                this.gameObject.layer = LayerMask.NameToLayer("Untargetable");
            else 
                this.gameObject.layer = LayerMask.NameToLayer("Player");

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

      


         private void TickTimeEchoSkill()
        {
            if (!SkillButtonHandler.TryConsumeEffect((int)ButtonSkillName.TimeEcho)) return;
            ServiceLocator.Get<PlayerSkillManager>()?.skillTimeEcho.ExecuteSkillEffect();
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
