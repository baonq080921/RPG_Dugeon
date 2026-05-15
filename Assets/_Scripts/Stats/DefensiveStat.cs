using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "DefensiveStat", menuName = "RPG/DefensiveStat")]
    //This data stats only use for player 
    public class DefensiveStats : ScriptableObject
    {


        [field: SerializeField] public Stat MaxHealth { get; private set; }
        [field:SerializeField] public Stat Envasion { get; private set; }

    }
}