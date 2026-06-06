using UnityEngine;

public class SkillAnimationEvent : MonoBehaviour 
{

    [SerializeField] private SkillObject_Base skillObject_Base;


    public void AttackTrigger()
    {
        skillObject_Base.AttackTrigger();
    }    
    public void EndOfAnimation()
    {
        skillObject_Base.HandleDeath();
    }
}