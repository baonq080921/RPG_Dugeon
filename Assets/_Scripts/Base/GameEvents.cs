using System;
using UnityEngine;

namespace Base
{
    #region InGameEvent
    /// <summary>Raised when the player's health reaches zero and the death state is entered.</summary>
    public struct PlayerDiedEvent : IEvent { }
    public struct PlayerCheckPointEvent : IEvent { }

    /// <summary>Raised when an enemy's health reaches zero. Carries the enemy's level and base XP for the reward formula.</summary>
    public struct OnEnemyDiedEvent : IEvent
    {
        public int EnemyLevel { get; }
        public float BaseExp   { get; }
        public OnEnemyDiedEvent(int enemyLevel, float baseExp)
        {
            EnemyLevel = enemyLevel;
            BaseExp    = baseExp;
        }
    }

    /// <summary>Raised whenever the player's XP value changes — on gain, level-up carry-over, or save restore.</summary>
    public struct PlayerXPChangedEvent : IEvent
    {
        public float CurrentExp     { get; }
        public float ExpToNextLevel { get; }
        public int   Level          { get; }
        public PlayerXPChangedEvent(float currentExp, float expToNextLevel, int level)
        {
            CurrentExp     = currentExp;
            ExpToNextLevel = expToNextLevel;
            Level          = level;
        }
    }

    public struct ResetStats : IEvent { }
    public struct EndGameEvent : IEvent { }
    #endregion

    #region Boosting Event
    public struct PlayerAddHealthAmountEvent : IEvent
    {
        public float Amount { get; }
        public PlayerAddHealthAmountEvent(float amount)
        {
            Amount = amount;
        }
    }

    public struct PlayerBoostingAmountEvent : IEvent
    {
        public float Amount { get; }
        public PlayerBoostingAmountEvent(float amount)
        {
            Amount = amount;
        }
    }
    #endregion

    #region UIEvent
    public struct OnInventoryChangedEvent : IEvent { }

    public struct OnToggleButtonUIEvent : IEvent
    {
        public bool isShow;
        public OnToggleButtonUIEvent(bool isShow)
        {
            this.isShow = isShow;
        }
    }

    public struct CraftStoreCallEvent : IEvent { }
    public struct StoreCallEvent : IEvent { }

    /// <summary>Raised when we want to alert some message.</summary>
    public struct AlertNotiEvent : IEvent
    {
        public string alertMessage { get; private set; }
        public Vector2 position;
        public Color color;
        public AlertNotiEvent(String message, Vector2 position, Color color = default)
        {
            alertMessage   = message;
            this.position  = position;
            this.color     = color;
        }
    }
    #endregion

    #region Quest Event
    /// <summary>Raised when the player rescues (interacts with) a rescuable NPC.</summary>
    public struct NpcRescuedEvent : IEvent { }

    /// <summary>Raised by any object that wants to grant the player skill points (chests, quest rewards, etc.).</summary>
    public struct SkillPointRewardEvent : IEvent
    {
        public float Amount { get; }
        public SkillPointRewardEvent(float amount) => Amount = amount;
    }

    public struct MoneyAddRewardEvent : IEvent
    {
        public float Amount { get; }
        public MoneyAddRewardEvent(float amount) => Amount = amount;
    }
    #endregion

    /// <summary>Raised when the player enters a new camera zone, carrying the new bounding shape.</summary>
    public struct CameraZoneChangedEvent : IEvent
    {
        public Collider2D BoundingShape { get; }
        public CameraZoneChangedEvent(Collider2D shape) => BoundingShape = shape;
    }

    /// <summary>Raised when the game pause state changes.</summary>
    public struct GamePauseChangedEvent : IEvent
    {
        public bool IsPause;
        public GamePauseChangedEvent(bool isPause) => IsPause = isPause;
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

    /// <summary>Raised by SaveManager after scene states are restored so SceneEntityManager can re-apply persistence.</summary>
    public struct SceneStateRestoredEvent : IEvent { }

    public struct DamagePopupEvent : IEvent
    {
        public Vector3 WorldPosition { get; }
        public float Damage          { get; }
        public bool IsCrit           { get; }
        public DamagePopupEvent(Vector3 worldPosition, float damage, bool isCrit)
        {
            WorldPosition = worldPosition;
            Damage        = damage;
            IsCrit        = isCrit;
        }
    }
}
