using System;
using System.Collections.Generic;

namespace Save
{
    /// <summary>Skill tree state written to / read from the save file.</summary>
    [Serializable]
    public class SkillTreeSaveData
    {
        public float skillPoints;
        public List<NodeSaveEntry> nodes = new List<NodeSaveEntry>();
    }
}
