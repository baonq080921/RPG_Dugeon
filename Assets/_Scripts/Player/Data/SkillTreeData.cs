
using player;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillTree-", menuName = "RPG/Skill Tree Data")]
public class SkillTreeData : ScriptableObject {
    [field:SerializeField] public string SkillTreeId  { get; private set; }
    [field:SerializeField] public SkillType SkillType {get; private set;}
    [field:SerializeField] public UpgradeData UpgradeData {get; private set;}
    [field:SerializeField] public string skillTreeDataName { get; private set; }
    [field:TextArea(3,10)]
    [field:SerializeField] public string Description { get; private set; }
    [field:SerializeField] public Sprite Icon { get; private set; }
    [field:SerializeField] public float  Cost { get; private set; }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(SkillTreeId))
        {
            SkillTreeId = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
[System.Serializable]
public class UpgradeData
{
    [field:SerializeField] public SkillUpgrade SkillUpgrade {get; private set;}
    [field:SerializeField] public float UpgradeCoolDown {get; private set;}
    [field:SerializeField] public float UpgradeDuration {get; private set;}
    [Tooltip("Additional damage added to the base skill damage. Set to 0 for no change.")]
    [field:SerializeField] public float UpgradeDamage {get; private set;} = 0f;
    [field: SerializeField] public float UpgradeHealing { get; private set; } = 0f;
    [field: SerializeField] public float UpgradeCD { get; private set; } = 0f;

}
