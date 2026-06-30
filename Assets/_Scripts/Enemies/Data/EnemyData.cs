using UnityEngine;

namespace enemy
{
    /// <summary>Per-enemy stat sheet.</summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "RPG/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [field:Header("Time stuff")]
        [field: SerializeField] public float IdleTime { get; private set; } = 2f;
        [field: SerializeField] public float AttackCooldown { get; private set; } = 1.5f;
        [field:Header("SKILL UlTIMATE DATA")]
        [field:SerializeField] public float SkillCoolDown {get; private set;}
        [field:SerializeField] public float SkillDuration{get; private set;}
        [field:SerializeField] public float SkillSpawnInterval {get; private set;} = 0.1f;
        [field:SerializeField] public float SkillDamageFactor {get; private set;} = 1f;

        [field: Header("Attack Velocity")]
        [field:SerializeField] public Vector2 AttackVelocityRetreat {get; private set;}
        

        [field: Header("Movement")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [field: Range(1, 2)]
        [field: SerializeField] public float MoveMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float JumpForce { get; private set; } = 10f;

        [field: Header("Level & Experience")]
        [field: SerializeField] public int Level { get; private set; } = 1;
        [field: SerializeField] public float BaseExp { get; private set; } = 50f;
        [field:Header("Gold/Money & SkillPoint")]
        [field:SerializeField] public float SkillPoint {get; private set;} = 1f;
        [field:SerializeField] public float Gold {get; private set; } = 10f;

        [field: Header("Detection")]
        [field: SerializeField] public float minDistanceRetreat { get; private set; } = 1f;

        [field: SerializeField] public float DetectionRange { get; private set; } = 7f;
        [field: SerializeField] public float AttackRange { get; private set; } = 1.5f;
        [field: SerializeField] public LayerMask WhatIsPlayer { get; private set; }
        [field: SerializeField] public Material Material {get; private set;}

    }
}
