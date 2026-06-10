

using Base;
using Stats;
using UnityEngine;
public enum EntityType
{
    None,
    PlayerNormal,
    PlayerSpecial,
    Enemy
}

public class EntityStat : MonoBehaviour
{
    [SerializeField]private EntityType _entityType;
    [SerializeField] private MajorStats _majorStats;
    //[SerializeField] private OffensiveStats _offensiveStats;
    [SerializeField] private DefensiveStats _defensiveStats;
    [SerializeField] private OffensiveStats _offensiveStats;
    private const float K = 100f; //Scaling factor for diminishing returns on armor
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

    public float GetElementalDamageValue(out ElementType elementType)
    {
        float fireDamage = _offensiveStats.fireDamage.GetValue();
        float lightDamage = _offensiveStats.lightDamage.GetValue();
        float bonusElementalDamage = _majorStats.Intelligence.GetValue(); // with each intelligence point increase they will bonus 0.5 % elemental damage
        float totalFireDamage = fireDamage + bonusElementalDamage;
        float totalLightDamage = lightDamage + bonusElementalDamage;

        if(_entityType == EntityType.PlayerNormal)
        {
            elementType = ElementType.Electric;
            return totalLightDamage; // Player normal attack will only deal light damage
        }

        else if(_entityType == EntityType.PlayerSpecial)
        {
            elementType = ElementType.Fire;
            return totalFireDamage + totalLightDamage * 0.5f; //player special attack will deal both fire and light damage but light damage will be reduced by 50%
        }

        else 
        {
                elementType = ElementType.None;
            return 0f; // Enemies do not deal elemental damage in this design
        }
    }




    public float GetElementalResitanceValue()
    {
        float baseResitance = _defensiveStats.ElementalResitance.GetValue();
        float bonusResitance = _majorStats.Intelligence.GetValue() * 0.5f; // with each intelligence point increase they will bonus 0.5 % elemental resitance
        float totalResitance = baseResitance + bonusResitance;
        float mitigation = K / (K + totalResitance); // Diminishing returns formula
        return mitigation;
    }
    public float GetMigiationValue()
    {

        float baseAmor = _defensiveStats.Amor.GetValue();
        float bonusAmor = _majorStats.Vitality.GetValue(); // with each vitality point increase they will bonus 1 armor
        // Calculate total armor and apply diminishing returns
        float totalAmor = baseAmor + bonusAmor;
        float mitigation = K / (K + totalAmor); // Diminishing returns formula
        return mitigation;
    }
    
  
    public float GetKnockBackThreshHold()
    {
        return _defensiveStats.KnockBackThreshHold.GetValue();
    }


    public float GetAttackMultiplier()
    {
        float baseAttackSpeed = _offensiveStats.AttackMultiplier.GetValue();
        float bonusAttackSpeed = _majorStats.Agility.GetValue() * 0.5f; // with each Agility point increase they will bonus 0.5 % attack speed
        float totalAttackSpeed = baseAttackSpeed + bonusAttackSpeed;
        return totalAttackSpeed;
    }

    public float GetHealthRegen()
    {
        float maxHealth = GetHealthValue();
        float baseRegen = _defensiveStats.HealthRegen.GetValue();
        float regenCap = maxHealth * 0.05f;
        return baseRegen > regenCap ? regenCap : baseRegen;
    }




    [ContextMenu("Reset all the stats to default")]

    /// <summary>
    /// Stats for Player Only
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public void ResetAllStats()
    {
        _majorStats.Strength.Reset();
        _majorStats.Agility.Reset();
        _majorStats.Intelligence.Reset();
        _majorStats.Vitality.Reset();

        _offensiveStats.Damage.Reset();
        _offensiveStats.CritChance.Reset();
        _offensiveStats.CritPower.Reset();
        _offensiveStats.AttackMultiplier.Reset();
        _offensiveStats.fireDamage.Reset();
        _offensiveStats.lightDamage.Reset();

        _defensiveStats.MaxHealth.Reset();
        _defensiveStats.Amor.Reset();
        _defensiveStats.Envasion.Reset();
        _defensiveStats.ElementalResitance.Reset();
        _defensiveStats.KnockBackThreshHold.Reset();
        _defensiveStats.HealthRegen.Reset();
    }

    public Stat GetStatByType(StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength: 
                return _majorStats.Strength;
            case StatType.Agility: 
                return _majorStats.Agility;
            case StatType.Intelligence: 
                return _majorStats.Intelligence;
            case StatType.Vitality: 
                return _majorStats.Vitality;
            case StatType.Damage: 
                return  _offensiveStats.Damage;
             case StatType.CritChance: 
                return  _offensiveStats.CritChance;
            case StatType.CritPower: 
                return  _offensiveStats.CritPower;
            case StatType.ElementalDamage: 
                return  _offensiveStats.lightDamage;
            case StatType.AttackMultiplier:
                return _offensiveStats.AttackMultiplier;
            case StatType.HealthRegen:
                return _defensiveStats.HealthRegen;
            case StatType.Envasion:
                return _defensiveStats.Envasion;
            case StatType.Armor:
                return _defensiveStats.Amor;
        }
        return null ;
    }
}
