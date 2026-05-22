using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "OffensiveStats", menuName = "RPG/OffensiveStats")]
    //This data stats only use for player 
    public class OffensiveStats : ScriptableObject
    {

        [field:Header("Physical Damage")]
        [field: SerializeField] public Stat Damage { get; private set; }
        [field: SerializeField] public Stat CritPower { get; private set; }
        [field: SerializeField] public Stat CritChance { get; private set; }
        [field: SerializeField] public Stat AttackMultiplier { get; private set; }
        [field:Space(3)]
        
        [field:Header("Elemental Damage")]
        [field:SerializeField] public Stat fireDamage { get; private set; }
        [field: SerializeField] public Stat lightDamage { get; private set; }

    }
}