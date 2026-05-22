using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "DefensiveStat", menuName = "RPG/DefensiveStat")]
    //This data stats only use for player 
    public class DefensiveStats : ScriptableObject
    {

        [field: SerializeField] public Stat KnockBackThreshHold { get; private set; }
        [field: SerializeField] public Stat MaxHealth { get; private set; }
        [field:SerializeField] public Stat Envasion { get; private set; }
        [field: SerializeField] public Stat Amor { get; private set; }
        [field: SerializeField] public Stat ElementalResitance { get; private set; }

        [field: SerializeField] public Stat HealthRegen { get; private set; }
    }
}