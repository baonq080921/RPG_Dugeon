using UnityEngine;

namespace player
{
    /// <summary>
    /// Per-character stat sheet. Create one asset per character via
    /// Create → RPG → Character Data, then assign it to the Player prefab's Data field.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterData", menuName = "RPG/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [field: Header("Movement")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [field: SerializeField] public float JumpForce { get; private set; } = 10f;
        [field: SerializeField] public int MaxJumpCount { get; private set; } = 2;
        [field: SerializeField, Range(0f, 1f)] public float AirControlFactor { get; private set; } = 1f;
        [field: SerializeField, Range(0f, 1f)] public float SlideDownSpeed { get; private set; } = 0.5f;
        [field: SerializeField] public Vector2 WallJumpForce { get; private set; }

        [field: Header("Dash")]
        [field: SerializeField] public float DashSpeed { get; private set; } = 15f;
        [field: SerializeField] public float DashDuration { get; private set; } = 0.2f;
        [field: SerializeField] public float DashCooldown { get; private set; } = 1f;

        [field: Header("Attack")]
        [field: SerializeField] public Vector2[] AttackVelocities { get; private set; }
        [field: SerializeField] public int ComboLimit { get; private set; } = 3;
        [field: SerializeField, Range(0f, 1f)] public float TimeResetCombo { get; private set; } = 0.2f;
        [field: SerializeField] public float ComboEndDelay { get; private set; } = 0.8f;
    }
}
