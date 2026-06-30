using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemCrafting-", menuName = "RPG/ItemData/ItemCrafting")]
public class ItemCraftData : EquipmentData 
{
    [field:SerializeField] public RequirementItem[] requirementItems;
    
}

[System.Serializable]
public class RequirementItem
{
    public ItemData itemData;
    public float amount;
}