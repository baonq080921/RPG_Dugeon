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

        [field: Header("KnockBack")]
        [field: SerializeField] public Vector2 KnockBack { get; private set; }
        [field: SerializeField] public float KnockBackDuration { get; private set; } = 0.3f;

        [field: Header("Attack Velocity")]
        [field: SerializeField] public Vector2[] AttackVelocities { get; private set; }
        [field: SerializeField] public int ComboLimit { get; private set; } = 3;
        [field: SerializeField, Range(0f, 1f)] public float TimeResetCombo { get; private set; } = 0.2f;
        [field: SerializeField] public float ComboEndDelay { get; private set; } = 0.8f;
        [field: SerializeField] public float AirAttackCooldown { get; private set; } = 0.8f;
        [field : SerializeField] public LayerMask WhatisTarget;
        [field: SerializeField] public Material Material {get; private set;}

        // Cached defaults so boosts can be rolled back via ResetBoosts.
        // ScriptableObject fields persist between Play sessions in the editor, so we snapshot
        // the inspector-authored values on first boost rather than in OnEnable.
        private float _defaultMoveSpeed;
        private float _defaultJumpForce;
        private int _defaultMaxJumpCount;
        private float _defaultDashSpeed;
        private bool _hasCachedDefaults;

        /// <summary>Adds <paramref name="amount"/> to <see cref="MoveSpeed"/>. Pass a negative value to reduce it.</summary>
        public void BoostMoveSpeed(float amount)
        {
            CacheDefaults();
            MoveSpeed += amount;
        }

        /// <summary>Adds <paramref name="amount"/> to <see cref="JumpForce"/>. Pass a negative value to reduce it.</summary>
        public void BoostJumpForce(float amount)
        {
            CacheDefaults();
            JumpForce += amount;
        }

        /// <summary>Adds <paramref name="amount"/> to <see cref="MaxJumpCount"/>. Pass a negative value to reduce it.</summary>
        public void BoostMaxJumpCount(int amount)
        {
            CacheDefaults();
            MaxJumpCount += amount;
        }

        /// <summary>Adds <paramref name="amount"/> to <see cref="DashSpeed"/>. Pass a negative value to reduce it.</summary>
        public void BoostDashSpeed(float amount)
        {
            CacheDefaults();
            DashSpeed += amount;
        }

        /// <summary>Restores <see cref="MoveSpeed"/>, <see cref="JumpForce"/>, <see cref="MaxJumpCount"/>, and <see cref="DashSpeed"/> to their inspector-authored defaults.</summary>
        public void ResetBoosts()
        {
            if (!_hasCachedDefaults) return;
            MoveSpeed = _defaultMoveSpeed;
            JumpForce = _defaultJumpForce;
            MaxJumpCount = _defaultMaxJumpCount;
            DashSpeed = _defaultDashSpeed;
        }

        private void CacheDefaults()
        {
            if (_hasCachedDefaults) return;
            _defaultMoveSpeed = MoveSpeed;
            _defaultJumpForce = JumpForce;
            _defaultMaxJumpCount = MaxJumpCount;
            _defaultDashSpeed = DashSpeed;
            _hasCachedDefaults = true;
        }
    }
}
