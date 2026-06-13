using System;

namespace Save
{
    /// <summary>One equipment slot.</summary>
    [Serializable]
    public class EquipSaveEntry
    {
        public string slotType;  // EquipSlotType enum name
        public string itemId;    // empty = slot is empty
    }
}
