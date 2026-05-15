using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "RPG/Entity Data")]
public class EntityData : ScriptableObject
{
    [field:Header("Data All Entity consist: ")]


    [field: Header("HP base value")]

    [field: SerializeField] public float MaxHealth { get; private set; } = 50f;



    [field: Header("Combat")]
    [field: SerializeField] public float Damage { get; private set; } = 5f;
    [field: SerializeField] public float StunDuration { get; private set; } = 0.3f;
    [field: SerializeField] public Vector2 KnockBack{get;private set;}
    [field:SerializeField] public float KnockBackDuration { get; private set;} = 0.15f;
   

    [field:Header("Base Move and Jump value")]
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
    [field: SerializeField] public float JumpForce { get; private set; } = 10f;
    [field: SerializeField] public LayerMask WhatIsTarget { get; private set; }






}