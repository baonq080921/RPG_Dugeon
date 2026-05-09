using stateMachine;
using UnityEngine;
public class EntityState
{
    protected StateMachine stateMachine;
    protected string animBoolName;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected bool isTriggered;

    public EntityState(StateMachine stateMachine,string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        animator.SetBool(animBoolName, true);
        isTriggered = false;
    }


    public virtual void Update()
    {
    }


    public virtual void Exit()
    {
        animator.SetBool(animBoolName, false);
    }



}