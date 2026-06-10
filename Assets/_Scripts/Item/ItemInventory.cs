using Stats;
using UnityEngine;
[System.Serializable]
public class ItemInventory 
{
    [field:SerializeField]public ItemData itemData {get; private set;}
    [field:SerializeField] public int stackSize{get;private set;} = 1 ;
    public ItemModifier[] modifiers {get;private set;}
    private int MaxStackize = 99;

    public ItemInventory(ItemData itemData)
    {
        this.itemData = itemData;
        modifiers = EquipmentData()?.modifiers;
    }

    public void AddModifiers(EntityStat playerStats)
    {
        foreach(var mod in modifiers)
        {
            Stat statModifier = playerStats.GetStatByType(mod.statType);
            statModifier.AddModifier(mod.value,itemData.name);
        }
    }

    public void RemoveModifiers(EntityStat playerStats)
    {
         foreach(var mod in modifiers)
        {
            Stat statModifier = playerStats.GetStatByType(mod.statType);
            statModifier.AddModifier(mod.value,itemData.name);
        }
    }

    private EquipmentData EquipmentData()
    {
        if(itemData is EquipmentData equipment)
            return equipment;
        return null;
    }

    public bool CanAddToStack() => stackSize < MaxStackize;

    public void AddStack()=> stackSize++;
    public void RemoveStackSize() => stackSize--;
}
