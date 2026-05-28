using Base;
using UnityEngine;
namespace player
{
    
    public class PlayerSkillManager : MonoBehaviour
    {
        public SkillDash skillDash {get; private set;}
        public SkillCounter skillCounter {get; private set;}

        void Awake()
        {
            skillDash = GetComponentInChildren<SkillDash>();
            skillCounter = GetComponentInChildren<SkillCounter>();
            ServiceLocator.Register(this);
        }

        public SkillBase GetSkillByType(SkillType type)
        {
            switch (type)
            {
                case SkillType.Dash: return skillDash;
                default: return null;
            }
        }
    }
}
