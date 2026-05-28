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

        public void SetToUpgradeDurationAndCoolDown(float newCool, float newDur)
        {
            Duration = newDur;
            Cooldown = newCool;
        }
    }
}
