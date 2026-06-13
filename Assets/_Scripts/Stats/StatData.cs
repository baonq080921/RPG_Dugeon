using System;
using UnityEngine;
public enum StatType
{
    Damage,
    CritChance,
    ElementalDamage,
    Armor,
    ElementalResitance,
    Strength,
    Agility,
    Intelligence,
    Vitality,
    KnockBackThreshHold,
    MaxHealth,
    Envasion,
    HealthRegen,
    AttackMultiplier,
    CritPower

}
namespace Stats
{
    [System.Serializable]
    public class Stat
    {
        public StatType statType;
        [SerializeField]private float baseValue;
        [SerializeField]private float value;

        public float GetValue() => value;
        public float GetBaseValue() => baseValue;
        public void AddModifier(float addValue, String name)
        {
            value += addValue;
        }
        public void RemoveModifier(float decreaseVal, String name)
        {
            value -= decreaseVal;
        }

        public void Reset() => value = baseValue;
    }
}