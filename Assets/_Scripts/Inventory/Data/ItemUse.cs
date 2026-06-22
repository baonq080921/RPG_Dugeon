using Stats;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemUse data -", menuName = "RPG/ItemData/UseItem Data")]
public class ItemUse :  ItemData
{   
    /// <summary>
    /// This Item data increase some thing like hp mana  expand in the future ,...
    /// </summary> <summary>
    /// 
    /// </summary>
    [Header("Item Modifier")]
    public float Amount; 
}

