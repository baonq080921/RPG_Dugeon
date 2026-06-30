using System;

namespace SaveData
{
    /// <summary>One bag-inventory slot.</summary>
    [Serializable]
    public class ItemSaveEntry
    {
        public string itemId;
        public int    stackSize;
    }
}
