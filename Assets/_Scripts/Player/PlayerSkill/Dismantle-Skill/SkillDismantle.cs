using System.Collections;
using player;
using UnityEngine;

namespace player
{
    public class SkillDismantle : SkillBase
    {
        [SerializeField] private SkillObjectDismantlePool _skillObjectDismantlePool;
        [SerializeField] private float _launchDelay = 0.3f;

        public override void ExecuteSkillEffect()
        {
            base.ExecuteSkillEffect();
            switch (skillUpgrade)
            {
                case SkillUpgrade.Dismantle:
                    _skillObjectDismantlePool.Get().Dismantle(player.transform);
                    break;
                case SkillUpgrade.Dismantle_Upgrade:
                    StartCoroutine(LaunchWithDelay(player.transform));

                    break;
            }
        }

        private IEnumerator LaunchWithDelay(Transform tf)
        {
            _skillObjectDismantlePool.Get().DismantleWithReturn(tf);
            yield return new WaitForSeconds(_launchDelay);
            _skillObjectDismantlePool.Get().DismantleWithReturn(tf);
        }

        public override void SetUpgradeForSkill(UpgradeData upgrade)
        {
            base.SetUpgradeForSkill(upgrade);
            SkillBaseDefinition.SetToUpgradeDamage(upgrade.UpgradeDamage);
            SkillBaseDefinition.SetToUpgradeCD(upgrade.UpgradeCD);
        }
    }
}
