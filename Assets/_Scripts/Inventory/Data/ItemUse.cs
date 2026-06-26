using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "ItemUse data -", menuName = "RPG/ItemData/UseItem Data")]
    public class ItemUse : ItemData
    {
        /// <summary>
        /// This Item data increase some thing like hp mana  expand in the future ,...
        /// </summary>
        [Header("Item Modifier")]
        public float Amount;
    }
}
