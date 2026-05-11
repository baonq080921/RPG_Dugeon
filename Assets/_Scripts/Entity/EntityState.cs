using stateMachine;
using UnityEngine;
public class EntityState
{
    protected StateMachine stateMachine;
    protected string animBoolName;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected bool isTriggered;
    protected float stateTimer;

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
        stateTimer -= Time.deltaTime;
    }


    public virtual void Exit()
    {
        animator.SetBool(animBoolName, false);
    }

    public void TriggerAnimation()
    {
        if (isTriggered) return;
        isTriggered = true;
    }

    

}