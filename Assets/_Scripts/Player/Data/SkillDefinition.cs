using UnityEngine;

namespace player
{
    /// <summary>
    /// Data asset for one skill slot: cooldown and icon.
    /// Create via Create → RPG → Skill Definition, then assign to <see cref="SkillManager"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "SkillDefinition", menuName = "RPG/Skill Definition")]
    public class SkillDefinition : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public float Duration { get; private set; } = 0.5f;
        [field: SerializeField] public float Cooldown { get; private set; } = 1f;
    }
}
