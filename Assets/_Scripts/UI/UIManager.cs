using Base;
using UnityEngine;

public class UIManager : MonoBehaviour
{   
    public UISkillToolTip uISkillToolTip { get; private set; }
    private void Awake() {
        if (uISkillToolTip == null)
            uISkillToolTip = GetComponentInChildren<UISkillToolTip>();
        ServiceLocator.Register(this);
    }
}
