using UnityEngine;
using stateMachine;
public abstract class Entity : MonoBehaviour
{
    public StateMachine stateMachine {get; private set;}
    public Animator animator {get; private set;}

    public Rigidbody2D rb {get; private set;}
    [SerializeField] protected LayerMask _whatIsGround;
    [SerializeField] protected LayerMask _whatIsWall;
    [SerializeField] protected Transform _groundCheckPoint;

    [Space]
    [Range(0,1f)]
    [SerializeField] protected float _groundCheckRadius = 0.1f;

    [Space]
    [Range(0,1f)]
    [SerializeField] protected float _wallCheckDistance = 0.5f;


    public float direction {get; protected set;} = 1f; // 1 for right, -1 for left

    public bool isGrounded{get; protected set;}
    public bool isTouchingWall {get; protected set;}



    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start(){}



    protected virtual void Update()
    {
        stateMachine.currentState.Update();
        CheckGrounded();
        CheckWall();
    }

    public void Flip(float direction)
    {
        transform.localScale = new Vector3(direction, 1f, 1f);
    }


    public void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _whatIsGround);
    }
    public void CheckWall()
    {
        isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * direction, _wallCheckDistance, _whatIsWall);
    }


    public void OnDrawGizmos()
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