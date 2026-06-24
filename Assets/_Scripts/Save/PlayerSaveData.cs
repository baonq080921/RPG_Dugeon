using System;
using System.Collections.Generic;

namespace Save
{
    /// <summary>All player state that gets written to / read from the save file.</summary>
    [Serializable]
    public class PlayerSaveData
    {
        public string sceneName;
        public float  posX;
        public float  posY;
        public float  currentHealth;
        public float currentExp;
        public int currentLevel;
        public float expToNextLevel;
        public int strengthPoints;
        public int agilityPoints;
        public int intelligencePoints;
        public int vitalityPoints;
        public float currentGolds;
        public List<ItemSaveEntry>  inventory            = new List<ItemSaveEntry>();
        public List<EquipSaveEntry> equipment            = new List<EquipSaveEntry>();
        public SkillTreeSaveData    skillTree            = new SkillTreeSaveData();
        public List<string>         completedQuestScenes = new List<string>();
        public List<SceneStateData>      sceneStates      = new List<SceneStateData>();
        public List<QuestProgressEntry>  questProgress    = new List<QuestProgressEntry>();
    }
}
