using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item-", menuName = "RPG/ItemData/Item")]
public class ItemData : ScriptableObject 
{
    [field:SerializeField] public string ItemId   { get; private set; }
    [field:SerializeField] public string ItemName {get; private set;}
    [field:SerializeField] public ItemTypes ItemType{get; private set;}
    [field:SerializeField] public EquipSlotType EquipSlot {get; private set;}
    [field:SerializeField] public Sprite Sprite {get; private set;}

    [field:Header("Effect for item that have effect")]
    [field:SerializeField]public ItemEffectData itemEffectData {get; private set;}

    [field:Range(0,1000)]
    [field:SerializeField] public float itemRare{get; private set;}
    [field:SerializeField] public float rareCap{get; private set;} = 65f;
    [field:Range(0,100)]
    [field:SerializeField] public float dropChance{get; private set;}
    [field:SerializeField] public float money {get; private set;}


    void OnValidate()
    {
        if (string.IsNullOrEmpty(ItemId))
        {
            ItemId = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        dropChance = GetDropChance();
    }
    public float GetDropChance()
    {
        float maxRareItem = 1000;
        dropChance = (maxRareItem - itemRare)/ maxRareItem * 100 + 0.1f;
        if(dropChance >= rareCap) dropChance = rareCap;
        return dropChance;
    }



    
}