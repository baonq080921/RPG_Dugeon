using UnityEngine;

[CreateAssetMenu(fileName = "Item-", menuName = "RPG/ItemData/Item")]
public class ItemData : ScriptableObject 
{
    [field:SerializeField] public string ItemName {get; private set;}
    [field:SerializeField] public ItemTypes ItemType{get; private set;}
    [field:SerializeField] public EquipSlotType EquipSlot {get; private set;}
    [field:SerializeField] public Sprite Sprite {get; private set;}
    
}