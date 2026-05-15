

using Stats;
using UnityEngine;

public class EntityStat : MonoBehaviour
{
    [SerializeField] private MajorStats _majorStats;
    //[SerializeField] private OffensiveStats _offensiveStats;
    [SerializeField] private DefensiveStats _defensiveStats;
    public float GetHealthValue()
    {
        float baseHealth = _defensiveStats.MaxHealth.GetValue();
        float bonusHealth = _majorStats.Vitality.GetValue() * 10;
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
}
