using Base;
using player;
using UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{   
    public UISkillToolTip uISkillToolTip;
    public UISkillTree uISkillTree;
    public UI_Inventory uIInventory;
    public UICanvasChange uICanvasChange;
    
    private void Awake()
    {
        if (ServiceLocator.Get<UIManager>() != null) return;

        if (uISkillToolTip == null)
            uISkillToolTip = GetComponentInChildren<UISkillToolTip>();
        if (uISkillTree == null)
            uISkillTree = GetComponentInChildren<UISkillTree>();
        if(uIInventory == null)
            uIInventory = GetComponentInChildren<UI_Inventory>();
        if(uICanvasChange == null)
            uICanvasChange = GetComponentInChildren<UICanvasChange>();
        ServiceLocator.Register(this);
    }
}
