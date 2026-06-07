using UnityEngine;

public class SkillAnimationEvent : MonoBehaviour 
{

    [SerializeField] private SkillObject_Base skillObject_Base;
    public void AttackTrigger() // Animation event for side kick 1
    {
        skillObject_Base.AttackTrigger();
    }    

    public void AttackTriggerWithKnockBack()
    {
        skillObject_Base.AttackTrigger(true);
    }
    public void EndOfAnimation()
    {
        skillObject_Base.HandleDeath();
    }
}