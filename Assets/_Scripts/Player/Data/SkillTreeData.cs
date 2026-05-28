
using player;
using UnityEngine;
[CreateAssetMenu(fileName = "SkillTree-", menuName = "RPG/Skill Tree Data")]
public class SkillTreeData : ScriptableObject {
    [field:SerializeField] public SkillType SkillType {get; private set;}
     [field:SerializeField] public UpgradeData UpgradeData {get; private set;}
    [field: SerializeField] public string skillTreeDataName { get; private set; }
    [field:TextArea(3,10)]
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public float  Cost { get; private set; }
    
} 
[System.Serializable]
public class UpgradeData
{
    [field:SerializeField] public SkillUpgrade SkillUpgrade {get; private set;}
    [field:SerializeField] public float UpgradeCoolDown {get; private set;}
    [field:SerializeField] public float UpgradeDuration {get; private set;}
}
