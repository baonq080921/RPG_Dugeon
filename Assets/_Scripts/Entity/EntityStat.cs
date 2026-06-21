

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

    private void Awake() => ResetAllStats();

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
        float lightDamage = _offensiveStats.ElementalDamage.GetValue();
        float bonusElementalDamage = _majorStats.Intelligence.GetValue(); // with each intelligence point increase they will bonus 0.5 % elemental damage
        float totalLightDamage = lightDamage + bonusElementalDamage;

        if(_entityType == EntityType.PlayerNormal)
        {
            elementType = ElementType.Electric;
            return totalLightDamage; // Player normal attack will only deal light damage
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
    /// <summary>
    /// THe higher the Migitaion the player more defensive invincible
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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




    #region Display Values (deterministic, no randomization — caps match combat methods)
    //This server the same as all the get but is server only for display all the stats that player define in the SO file
    //example attack, amor, critchance etc...
    public float GetDamageDisplayValue() => _offensiveStats.Damage.GetValue() + _majorStats.Strength.GetValue();
    public float GetCritChanceDisplayValue()
    {
        float total = _offensiveStats.CritChance.GetValue() + _majorStats.Agility.GetValue() * 0.3f;
        return total > 100f ? 100f : total;
    }
    public float GetCritPowerDisplayValue() => _offensiveStats.CritPower.GetValue() + _majorStats.Strength.GetValue() * 0.5f;
    public float GetArmorDisplayValue() => _defensiveStats.Amor.GetValue() + _majorStats.Vitality.GetValue();
    public float GetElementalDamageDisplay() => _offensiveStats.ElementalDamage.GetValue() + _majorStats.Intelligence.GetValue();
    public float GetElementalResistanceDisplayValue() => (1f - GetElementalResitanceValue()) * 100f;
    #endregion

    /// <summary>
    /// Stats for Player Only
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    private int _strengthPoints;
    private int _agilityPoints;
    private int _intelligencePoints;
    private int _vitalityPoints;

    public int GetAllocatedPoints(StatType statType) => statType switch
    {
        StatType.Strength     => _strengthPoints,
        StatType.Agility      => _agilityPoints,
        StatType.Intelligence => _intelligencePoints,
        StatType.Vitality     => _vitalityPoints,
        _                     => 0
    };

#if UNITY_EDITOR
    [ContextMenu("Reset All Stats")]
    public void ResetAllStats()
    {
        _strengthPoints     = 0;
        _agilityPoints      = 0;
        _intelligencePoints = 0;
        _vitalityPoints     = 0;

        if (_majorStats != null)
        {
            _majorStats.Strength.Reset();
            _majorStats.Agility.Reset();
            _majorStats.Intelligence.Reset();
            _majorStats.Vitality.Reset();
        }

        if (_offensiveStats != null)
        {
            _offensiveStats.Damage.Reset();
            _offensiveStats.CritChance.Reset();
            _offensiveStats.CritPower.Reset();
            _offensiveStats.AttackMultiplier.Reset();
            _offensiveStats.ElementalDamage.Reset();
        }

        if (_defensiveStats != null)
        {
            _defensiveStats.MaxHealth.Reset();
            _defensiveStats.Amor.Reset();
            _defensiveStats.Envasion.Reset();
            _defensiveStats.ElementalResitance.Reset();
            _defensiveStats.KnockBackThreshHold.Reset();
            _defensiveStats.HealthRegen.Reset();
        }
    }

    #endif

    /// <summary>Permanently adds 1 point to the chosen major stat. Called by the level-up UI.</summary>
    public void AddMajorStatPoint(StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength:     StrenghLevelUp();     _strengthPoints++;     return;
            case StatType.Agility:      AgilityLevelUp();     _agilityPoints++;      return;
            case StatType.Intelligence: IntelegenceLevelUp(); _intelligencePoints++; return;
            case StatType.Vitality:     VitalityLevelUp();    _vitalityPoints++;     return;
        }
    }


    private void StrenghLevelUp()
    {
        _offensiveStats.Damage.AddModifier(1,"damage");
        _offensiveStats.CritPower.AddModifier(0.5f,"critPower");
    }
    private void AgilityLevelUp()
    {
        _defensiveStats.Envasion.AddModifier(0.5f,"envasion");
        _offensiveStats.CritChance.AddModifier(0.3f,"critChance");
    }
    private void IntelegenceLevelUp()
    {
        _offensiveStats.ElementalDamage.AddModifier(1,"magicPoint");
        _defensiveStats.ElementalResitance.AddModifier(0.5f,"MagicResitance");
    }


    private void VitalityLevelUp()
    {
        _defensiveStats.MaxHealth.AddModifier(5,"healPoint");
        _defensiveStats.Amor.AddModifier(1f,"Amor");
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
                return  _offensiveStats.ElementalDamage;
            case StatType.AttackMultiplier:
                return _offensiveStats.AttackMultiplier;
            case StatType.HealthRegen:
                return _defensiveStats.HealthRegen;
            case StatType.Envasion:
                return _defensiveStats.Envasion;
            case StatType.Armor:
                return _defensiveStats.Amor;
            case StatType.MaxHealth:
                return _defensiveStats.MaxHealth;
            case StatType.ElementalResitance:
                return _defensiveStats.ElementalResitance;
            case StatType.KnockBackThreshHold:
                return _defensiveStats.KnockBackThreshHold;
        }
        return null;
    }
}
