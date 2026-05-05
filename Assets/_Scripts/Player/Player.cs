using System;
using System.Collections;
using Base;
using stateMachine;
using UnityEngine;
using UnityEngine.Events;

namespace player
{
    public class Player : MonoBehaviour
    {

        [Header("Player Settings Jumps")]
        public float moveSpeed = 5f;
        public float jumpForce = 10f;

        [Header("player Settings Dash")]

        public float dashSpeed = 15f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;
        public bool canDash{get; private set;} = true;

        /// <summary>Fired when dash cooldown begins. Parameter is the total cooldown duration.</summary>
        public event Action<float> DashCooldownStarted;
        public void SetCanDash(bool value) => canDash = value;
        private float _dashCooldownTimer;

        [Header("Player Settings Attack")]
        public Vector2[] attackVelocity;
        public float timeResetCombo = 0.2f;
        public float comboEndDelay = 0.8f;
        public bool canAttack { get; private set; } = true;
        private float _attackCooldownTimer;
        [Space]
        [Range(0,1f)]
        public float coyoteTime = 0.2f;
        public int MaxJumpCount = 2;
        [Range(0f,1f)]
        public float slideDownSpeed = 0.5f;
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private LayerMask _whatIsWall;
        [SerializeField] private Transform _groundCheckPoint;
        [Range(0,1f)]
        [SerializeField] private float _groundCheckRadius = 0.1f;
        [Range(0,1f)]
        [SerializeField] private float _wallCheckDistance = 0.5f;

        public float direction {get; private set;} = 1f; // 1 for right, -1 for left
        public Vector2 WallJumpForce ;


        public PlayerInputSet input{get; private set;}
       
        private StateMachine _stateMachine;
        public Animator animator {get; private set;}

        public Rigidbody2D rb {get; private set;}


        

        public PlayerIdleState playerIdleState {get; private set;}

        public PlayerMoveState playerMovementState {get; private set;}

        public PlayerJumpState playerJumpState {get; private set;}
        public PlayerFallState playerFallState {get; private set;}
        public PlayerWallSildeState playerWallSlideState {get; private set;}
        public PlayerWallJumpState playerWallJumpState {get; private set;}
        public PlayerDashState playerDashState {get; private set;}
        public PlayerAttackState playerBasicAttackState {get; private set;}
        public PlayerJumpAttackState playerJumpAttackState {get; private set;}
        public Vector2 movementInput {get; private set;}
        public bool isJump { get; private set;}
        public bool JumpJustPressed { get; private set; }
        public bool DashJustPressed { get; private set; }
        public int JumpCount { get; set; }

        public bool isGrounded{get; private set;}
        public bool isTouchingWall {get; private set;}
        public float LastWallJumpDirection { get; set; } = 0f;


        private Coroutine _queueComboCouroutine;
        void Awake()
        {
            Application.targetFrameRate = 60;
            _stateMachine = new StateMachine();


            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();





            input = new PlayerInputSet();
            playerIdleState = new PlayerIdleState(this,_stateMachine,"Idle");
            playerMovementState = new PlayerMoveState(this,_stateMachine,"Move");
            playerJumpState = new PlayerJumpState(this,_stateMachine,"JumpFall"); 
            playerFallState = new PlayerFallState(this,_stateMachine,"JumpFall");
            playerWallSlideState = new PlayerWallSildeState(this,_stateMachine,"WallSlide");   
            playerWallJumpState = new PlayerWallJumpState(this,_stateMachine,"JumpFall");
            playerDashState = new PlayerDashState(this,_stateMachine,"Dash");
            playerBasicAttackState = new PlayerAttackState(this,_stateMachine,"BasicAttack");
            playerJumpAttackState = new PlayerJumpAttackState(this, _stateMachine,"BasicAttack");
        }


        void OnEnable()
        {
            input.Enable();
            input.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();  
            input.Player.Movement.canceled += ctx => movementInput = Vector2.zero;

            input.Player.Jump.performed += ctx => { isJump = true; JumpJustPressed = true; };
            input.Player.Jump.canceled += ctx => isJump = false;
            input.Player.Dash.performed += ctx => { DashJustPressed = true;};
        }

        void OnDisable()
        {
            input.Disable();            
        }



        void Start()
        {
            _stateMachine.Initialize(playerIdleState);
        }

        void Update()
        {
            CheckGrounded();
            CheckWall();
            TickDashCooldown();
            TickAttackCooldown();
            _stateMachine.currentState.Update();
        }

        public void EnterAttackComboCoroutine(){
            if(_queueComboCouroutine != null)
                StopCoroutine(_queueComboCouroutine);
            _queueComboCouroutine = StartCoroutine(AttackQueueCouroutine());
        }

        IEnumerator AttackQueueCouroutine(){
            yield return new WaitForEndOfFrame();
                _stateMachine.ChangeState(playerBasicAttackState);
        }

        public void StartDashCooldown()
        {
            canDash = false;
            _dashCooldownTimer = dashCooldown;
            DashCooldownStarted?.Invoke(dashCooldown);
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
            _attackCooldownTimer = comboEndDelay;
        }

        private void TickAttackCooldown()
        {
            if (_attackCooldownTimer <= 0) return;
            _attackCooldownTimer -= Time.deltaTime;
            if (_attackCooldownTimer <= 0)
                canAttack = true;
        }


        /// <summary>
        /// Clears JumpJustPressed after it has been consumed by a state.
        /// </summary>
        public void ConsumeJump() => JumpJustPressed = false;

        public void ConsumeDash() => DashJustPressed = false;   

        public void SetVelocity(Vector2 velocity)
        {
            rb.velocity = velocity;
            HandleFlip(movementInput.x);
        }


        public void HandleFlip(float horizontalInput)
        {
            if(horizontalInput > 0f)
            {
                direction = 1f;
                Flip(direction);
            }
            else if(horizontalInput < 0f){
                direction = -1f;
               Flip(direction);
            }
        }

        public void Flip(float direction)
        {
            transform.localScale = new Vector3(direction, 1f, 1f);
        }
        


        public void CheckGrounded()
        {
            isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _whatIsGround);
        }
        public void CheckWall(){
            isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * direction, _wallCheckDistance, _whatIsWall);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position,_wallCheckDistance * Vector2.right * direction + (Vector2)transform.position);


            if(isGrounded){
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
                return;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);           
        }


    }
}
