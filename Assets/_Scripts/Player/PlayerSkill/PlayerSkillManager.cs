using Base;
using UnityEngine;
namespace player
{
    
    public class PlayerSkillManager : MonoBehaviour
    {
        public SkillDash skillDash {get; private set;}
        public SkillCounter skillCounter {get; private set;}
        public SkillTimeEcho skillTimeEcho {get; private set;}
        public SkillDismantle skillDismantle {get; private set;}
        public DomainExpasionSkill domainExpasionSkill {get; private set;}

        void Awake()
        {
            if (ServiceLocator.Get<PlayerSkillManager>() != null) return;

            skillDash = GetComponentInChildren<SkillDash>();
            skillCounter = GetComponentInChildren<SkillCounter>();
            skillTimeEcho = GetComponentInChildren<SkillTimeEcho>();
            skillDismantle = GetComponentInChildren<SkillDismantle>();
            domainExpasionSkill = GetComponentInChildren<DomainExpasionSkill>();
            ServiceLocator.Register(this);
        }

        public SkillBase GetSkillByType(SkillType type)
        {
            switch (type)
            {
                case SkillType.Dash: return skillDash;
                case SkillType.TimeEcho: return skillTimeEcho;
                case SkillType.Dismantle: return skillDismantle;
                case SkillType.Domain : return domainExpasionSkill;
                default: return null;
            }
        }
    }
}
