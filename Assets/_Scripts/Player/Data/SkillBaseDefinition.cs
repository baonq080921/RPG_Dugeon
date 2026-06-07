using System.Runtime.CompilerServices;
using UnityEngine;

namespace player
{
    /// <summary>
    /// Data asset for one skill slot: cooldown and icon.
    /// Create via Create → RPG → Skill Definition, then assign to <see cref="SkillButtonHandler"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "SkillDefinition-", menuName = "RPG/Skill Definition")]
    public class SkillBaseDefinition : ScriptableObject
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.5f;
        [field: SerializeField] public float Cooldown { get; private set; } = 1f;
        [field: SerializeField] public float Damage { get; private set; } = 0f;
        [field: SerializeField] public float HealingAmount { get; private set; } = 0f;
        [field:Range(0f,1f)]
        [field:SerializeField] public float CDAmountPercent{get; private set;} = 0f;
        [field:SerializeField] public float moveSpeed {get; private set;} = 0f;

        public void SetToUpgradeDurationAndCoolDown(float newCool, float newDur)
        {
            Duration = newDur;
            Cooldown = newCool;
        }
        public void SetToUpgradeDamage(float newDamage = 0f)
        {
            Damage = newDamage;
        }
        public void SetToUpgradeHealing(float newHealing = 0f)
        {
            HealingAmount = newHealing;
        }

        public void SetToUpgradeCD(float newCd = 0f)
        {
            CDAmountPercent = newCd;
        }
    }
}
