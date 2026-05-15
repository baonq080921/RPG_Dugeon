using UnityEngine;

namespace enemy
{
    /// <summary>Per-enemy stat sheet.</summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "RPG/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [field: SerializeField] public float IdleTime { get; private set; } = 2f;

        [field: Header("Combat")]
        [field: SerializeField] public float Damage { get; private set; } = 5f;
        [field: SerializeField] public bool CanKnockBackPlayer { get; private set; } = true;
        [field:SerializeField] public Vector2 AttackVelocityRetreat {get; private set;}
        [field: SerializeField] public float StunDuration {get; private set;}
        [field: SerializeField] public float KnockBackThreshHold{get; private set;} = 0.3f;
        [field : SerializeField] public Vector2 KnockBackPowerLight{get; private set;}
        [field: SerializeField] public Vector2 KnockBackPowerHeavy {get; private set;}

        [field: Header("Movement")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [field: Range(1, 2)]
        [field: SerializeField] public float MoveMultiplier { get; private set; } = 1.5f;
        [field: SerializeField] public float JumpForce { get; private set; } = 10f;

        [field: Header("Detection")]
        [field: SerializeField] public float minDistanceRetreat { get; private set; } = 1f;

        [field: SerializeField] public float DetectionRange { get; private set; } = 7f;
        [field: SerializeField] public float AttackRange { get; private set; } = 1.5f;
        [field: SerializeField] public LayerMask WhatIsPlayer { get; private set; }
        [field: SerializeField] public Material Material {get; private set;}

    }
}
