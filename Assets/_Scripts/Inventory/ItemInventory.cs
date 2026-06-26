using entity;
using Stats;
using UnityEngine;

namespace Inventory
{
    [System.Serializable]
    public class ItemInventory
    {
        [field: SerializeField] public ItemData itemData { get; private set; }
        [field: SerializeField] public int stackSize { get; private set; } = 1;
        public ItemModifier[] modifiers { get; private set; }
        private ItemEffectData itemEffect;
        private int MaxStackize = 99;

        public ItemInventory(ItemData itemData)
        {
            this.itemData = itemData;
            modifiers = EquipmentData()?.modifiers;
            itemEffect = itemData?.itemEffectData;
        }

        public void AddModifiers(EntityStat playerStats)
        {
            if (modifiers == null) return;
            foreach (var mod in modifiers)
            {
                Stat statModifier = playerStats.GetStatByType(mod.statType);
                statModifier.AddModifier(mod.value, itemData.name);
            }
        }

        public void RemoveModifiers(EntityStat playerStats)
        {
            if (modifiers == null) return;
            foreach (var mod in modifiers)
            {
                Stat statModifier = playerStats.GetStatByType(mod.statType);
                statModifier.RemoveModifier(mod.value, itemData.name);
            }
        }

        public void AddItemEffect(IItemEffectTarget target) => itemEffect?.Subscribe(target);
        public void RemoveItemEffect(IItemEffectTarget target) => itemEffect?.Unsubscribe(target);

        private EquipmentData EquipmentData()
        {
            if (itemData is EquipmentData equipment)
                return equipment;
            return null;
        }

        public bool CanAddToStack() => stackSize < MaxStackize;

        public void AddStack() => stackSize++;
        public void RemoveStackSize() => stackSize--;
    }
}
