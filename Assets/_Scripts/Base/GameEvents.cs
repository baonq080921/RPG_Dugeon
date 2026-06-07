using UnityEngine;
namespace Base
{
    /// <summary>Raised when the player's health reaches zero and the death state is entered.</summary>
    public struct PlayerDiedEvent : IEvent { }

    /// <summary>Raised when an enemy's health reaches zero.</summary>
    public struct EnemyDiedEvent : IEvent { }


    public struct PlayerAddHealthAmount : IEvent
    {
        public float Amount { get; }

        public PlayerAddHealthAmount(float amount)
        {
            Amount = amount;
        }
    }

    public struct TargetGotHitEvent : IEvent
    {
        public Transform target;
        public bool isCrit;
        public TargetGotHitEvent(Transform target, bool isCrit)
        {
            this.target = target;
            this.isCrit = isCrit;
        }
    }
}
