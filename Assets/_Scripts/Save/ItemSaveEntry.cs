using System;

namespace Save
{
    /// <summary>One bag-inventory slot.</summary>
    [Serializable]
    public class ItemSaveEntry
    {
        public string itemId;
        public int    stackSize;
    }
}
