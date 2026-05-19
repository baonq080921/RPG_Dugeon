

using Base;
using Stats;
using UnityEngine;

public class EntityStat : MonoBehaviour
{
    [SerializeField] private MajorStats _majorStats;
    //[SerializeField] private OffensiveStats _offensiveStats;
    [SerializeField] private DefensiveStats _defensiveStats;
    [SerializeField] private OffensiveStats _offensiveStats;
    [field: SerializeField] public float StunDuration {get; private set;} = 0.3f;

    public float GetHealthValue()
    {
        float baseHealth = _defensiveStats.MaxHealth.GetValue();
        float bonusHealth = _majorStats.Vitality.GetValue() * 10f;
        float maxHealth = baseHealth + bonusHealth;
        return maxHealth; 
    }


    public float GetEnvasionValue()
    {

        float baseEnvasion = _defensiveStats.Envasion.GetValue();
        float bonusEnvasion = _majorStats.Agility.GetValue() * 0.5f; // with each agilty point increase they will bonus 0.5 % envasion chance
        float totalEnvasion = baseEnvasion + bonusEnvasion;
        float envasionCap = 85f;
        float finalEnvasion = envasionCap < totalEnvasion ? envasionCap : totalEnvasion;
        return finalEnvasion;
    }


    public float GetPhysicalDamageValue(out bool isCrit)
    {
        // Calculate damage based on offensive stats and major stats
        float baseDamage = _offensiveStats.Damage.GetValue();
        float bonusDamage = _majorStats.Strength.GetValue(); // with each strength point increase they will bonus 2 damage
        // Calculate critical hit chance and power
        float critPowerBase = _offensiveStats.CritPower.GetValue();
        float bonusCritPower = _majorStats.Strength.GetValue() * 0.5f; // with each strength point increase they will bonus 0.5 % crit power

        float criteChanceBase = _offensiveStats.CritChance.GetValue();
        float bonusCritChance = _majorStats.Agility.GetValue() * 0.3f; // with each Agility point increase they will bonus 0.3 % crit chance

        float totalDamage = baseDamage + bonusDamage;
        float totalCritPower = critPowerBase + bonusCritPower;

        float totalCritChance = criteChanceBase + bonusCritChance;
        float critCap = 100f;
        totalCritChance = totalCritChance > critCap ? critCap : totalCritChance;
        float finalDamage = Random.Range(0f, 100f) < totalCritChance ? totalDamage + (totalCritPower / 100f) : totalDamage;
        isCrit = finalDamage > totalDamage;
        return finalDamage;
        
    }


    public float GetKnockBackThreshHold()
    {
        return _defensiveStats.KnockBackThreshHold.GetValue();
    }
}
