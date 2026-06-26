using System;

namespace Save
{
    /// <summary>One unlocked skill tree node, keyed by its stable SkillTreeData.SkillTreeId GUID.</summary>
    [Serializable]
    public class NodeSaveEntry
    {
        public string nodeKey;
    }
}
