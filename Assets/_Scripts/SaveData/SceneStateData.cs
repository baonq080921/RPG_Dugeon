using System;
using System.Collections.Generic;

namespace SaveData
{
    /// <summary>Tracks which scene entities have been permanently changed (enemies killed, chests opened, items collected, etc.) for one scene.</summary>
    [Serializable]
    public class SceneStateData
    {
        public string sceneName;
        public List<string> persistedEntityIds = new List<string>();
    }
}
