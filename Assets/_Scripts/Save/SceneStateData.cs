using System;
using System.Collections.Generic;

namespace Save
{
    /// <summary>Tracks which enemies were killed and which chests were opened in a single scene for the current session.</summary>
    [Serializable]
    public class SceneStateData
    {
        public string sceneName;
        public List<string> killedEnemyIds = new List<string>();
        public List<string> openedChestIds = new List<string>();
    }
}
