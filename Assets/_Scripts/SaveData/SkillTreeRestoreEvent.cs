using Base;

namespace SaveData
{
    /// <summary>
    /// Raised by SaveManager after a full load or transition restore so UISkillTree
    /// can rebuild its unlocked-node state without SaveManager depending on UI types.
    /// </summary>
    public struct SkillTreeRestoreEvent : IEvent
    {
        public SkillTreeSaveData SkillTree { get; }
        public float SkillPoints { get; }
        public SkillTreeRestoreEvent(SkillTreeSaveData skillTree, float skillPoints)
        {
            SkillTree    = skillTree;
            SkillPoints  = skillPoints;
        }
    }
}
