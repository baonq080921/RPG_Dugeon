using Stats;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "Equipment data -", menuName = "RPG/ItemData/EquipmentItem Data")]
    public class EquipmentData : ItemData
    {
        [Header("Item Modifier")]
        public ItemModifier[] modifiers;
    }


    [System.Serializable]
    public class ItemModifier
    {
        public StatType statType;
        public float value;
    }
}
