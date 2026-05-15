using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "MajorStat", menuName = "RPG/MajorStat")]
    //This data stats only use for player 
    public class MajorStats: ScriptableObject
    {
        [field: SerializeField] public Stat Strength { get; private set; }
        [field:SerializeField] public Stat Agility { get; private set; }
        [field: SerializeField] public Stat Intelligence { get; private set; }
        [field: SerializeField] public Stat Vitality { get; private set; }
    }
}