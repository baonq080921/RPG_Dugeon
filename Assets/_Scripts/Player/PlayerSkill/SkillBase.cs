using UnityEngine;
namespace player
{
    
    public class SkillBase : MonoBehaviour
    {
        [field:Header("General Details")]
        [field:SerializeField] public SkillBaseDefinition SkillBaseDefinition {get; private set;}
        [field:SerializeField] public SkillType skillType {get; private set;}
        [field:SerializeField] public SkillUpgrade skillUpgrade {get; private set;}

        protected virtual void Awake(){}

        public void SetSkillUpgradeType(SkillUpgrade upgrade)
        {
            skillUpgrade = upgrade;
        }

        public void SetUpgradeForSkill(UpgradeData upgrade)
        {
            SkillBaseDefinition.SetToUpgradeDurationAndCoolDown(upgrade.UpgradeCoolDown, upgrade.UpgradeDuration);
        }
    }
}