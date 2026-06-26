using UnityEngine;

namespace player
{
    public class SkillTimeEcho : SkillBase
    {
        [SerializeField] private SkillObjectTimeEchoPool _pool;
        protected override void Awake()
        {
            base.Awake();
            if (_pool == null) Debug.LogError("Time Echo skill missing reference to its object pool.");
        }

        public override void ExecuteSkillEffect()
        {
            if (!player.isGrounded) return;
            base.ExecuteSkillEffect();

            switch (skillUpgrade)
            {
                case SkillUpgrade.TimeEcho:
                    _pool.GetDefault().TimeEchoBase(player.transform);
                    break;
                case SkillUpgrade.TimeEcho_ExtraEchoAttack:
                    _pool.GetDefault().TimeEchoSideKickAttack(player.transform);
                    break;
                case SkillUpgrade.TimeEcho_ExtraEchoAttackMaho:
                    _pool.GetMahoraga().TimeEchoSideKickMahoragaAttack(player.transform);
                    break;
                case SkillUpgrade.TimeEcho_HealOnEcho:
                    _pool.GetDefault().TimeEchoHealing(player.transform);
                    break;
                case SkillUpgrade.TimeEcho_HealOnEchoAndDuration:
                    _pool.GetDefault().TimeEChoHealingAndCoolDownAllSkill(player.transform);
                    break;
            }
        }

        public override void SetUpgradeForSkill(UpgradeData upgrade)
        {
            base.SetUpgradeForSkill(upgrade);
            SkillBaseDefinition.SetToUpgradeDamage(upgrade.UpgradeDamage);
            SkillBaseDefinition.SetToUpgradeHealing(upgrade.UpgradeHealing);
            SkillBaseDefinition.SetToUpgradeCD(upgrade.UpgradeCD);
        }
    }
}
