using Base;
using UnityEngine;

public class UIManager : MonoBehaviour
{   
    public UISkillToolTip uISkillToolTip;
    public UISkillTree uISkillTree;
    
    private void Awake() {
        if (uISkillToolTip == null)
            uISkillToolTip = GetComponentInChildren<UISkillToolTip>();
        
        if(uISkillTree == null)
            uISkillTree = GetComponentInChildren<UISkillTree>();
            
        ServiceLocator.Register(this);
    }
}
