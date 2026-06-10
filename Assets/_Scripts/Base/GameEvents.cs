using System;
using UnityEngine;
namespace Base
{
    /// <summary>Raised when the player's health reaches zero and the death state is entered.</summary>
    public struct PlayerDiedEvent : IEvent { }

    /// <summary>Raised when an enemy's health reaches zero.</summary>
    public struct EnemyDiedEvent : IEvent { }
    public struct ResetStats :IEvent {}
    /// <summary>
    /// Raised when we want to Alert some Message
    /// </summary> <summary>
    /// 
    /// </summary>
    public struct AlertNotiEvent : IEvent
    {
        public string alertMessage{get; private set;}
        public AlertNotiEvent(String message)
        {
            alertMessage = message;
        }
    } 

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


    public struct EquipEvent : IEvent
    {
        public ItemInventory itemInventory;
        public EquipEvent(ItemInventory itemInventory)
        {
            this.itemInventory = itemInventory;
        }
    }

    public struct OnInventoryChangedEvent:IEvent{}

    /// <summary>Raised when the game pause state changes.</summary>
    public struct GamePauseChangedEvent : IEvent
    {
        public bool IsPause;
        public GamePauseChangedEvent(bool isPause) => IsPause = isPause;
    }
}
