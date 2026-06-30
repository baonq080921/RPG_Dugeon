using System;
using System.Collections.Generic;

namespace SaveData
{
    /// <summary>Skill tree state written to / read from the save file.</summary>
    [Serializable]
    public class SkillTreeSaveData
    {
        public List<NodeSaveEntry> nodes = new List<NodeSaveEntry>();
    }
}
