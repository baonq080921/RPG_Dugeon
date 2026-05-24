
using UnityEngine;
[CreateAssetMenu(fileName = "SkillNodeTreeData", menuName = "RPG/Skill Tree Data")]
public class SkillTreeData : ScriptableObject {

    [field: SerializeField] public string skillTreeDataName { get; private set; }
    [field:TextArea(3,10)]
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public float  Cost { get; private set; }
    
} 
