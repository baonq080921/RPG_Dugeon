using System;
using UnityEngine;
namespace Base
{
    /// <summary>Raised when the player's health reaches zero and the death state is entered.</summary>
    public struct PlayerDiedEvent : IEvent { }
    public struct PlayerCheckPointEvent:IEvent{}

    /// <summary>Raised when an enemy's health reaches zero. Carries the enemy's level and base XP for the reward formula.</summary>
    public struct EnemyDiedEvent : IEvent
    {
        public int EnemyLevel { get; }
        public float BaseExp   { get; }
        public EnemyDiedEvent(int enemyLevel, float baseExp)
        {
            EnemyLevel = enemyLevel;
            BaseExp    = baseExp;
        }
    }

    /// <summary>Raised when the player gains enough XP to level up. Carries the stat sheet so the UI can apply the chosen bonus.</summary>
    public struct PlayerLevelUpEvent : IEvent
    {
        public EntityStat PlayerStat { get; }
        public PlayerLevelUpEvent(EntityStat playerStat) => PlayerStat = playerStat;
    }

    /// <summary>Raised whenever the player's XP value changes — on gain, level-up carry-over, or save restore.</summary>
    public struct PlayerXPChangedEvent : IEvent
    {
        public float CurrentExp    { get; }
        public float ExpToNextLevel { get; }
        public int   Level         { get; }
        public PlayerXPChangedEvent(float currentExp, float expToNextLevel, int level)
        {
            CurrentExp     = currentExp;
            ExpToNextLevel = expToNextLevel;
            Level          = level;
        }
    }
    public struct ResetStats :IEvent {}

    //Call to Open the craft store
    public struct CraftStoreCallEvent:IEvent{}
    // Call to Open the store 
    public struct StoreCallEvent: IEvent{}
    /// <summary>
    /// Raised when we want to Alert some Message
    /// </summary> <summary>
    /// 
    /// </summary>
    public struct AlertNotiEvent : IEvent
    {
        public string alertMessage{get; private set;}
        public Vector2 position;
        public Color color;
        public AlertNotiEvent(String message, Vector2 position, Color color = default)
        {
            alertMessage = message;
            this.position = position;
            this.color = color;
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

    public struct CraftGetInfoEvent: IEvent
    {
        public ItemCraftData itemCraftData;
        public CraftGetInfoEvent(ItemCraftData itemCraftData)
        {
            this.itemCraftData = itemCraftData;
        }
    }

    public struct StoreItemGetInfoEvent : IEvent
    {
        public ItemData itemData;
        public StoreItemGetInfoEvent(ItemData itemData)
        {
            this.itemData = itemData;
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
    //When something change in the inventory for instance looting, or some stat upgrade that effect the ui stat on inventory
    public struct OnInventoryChangedEvent:IEvent{} 

    //When you open store
    public struct OnToggleButtonUIEvent:IEvent
    {
        public bool isShow;
        public OnToggleButtonUIEvent(bool isShow)
        {
            this.isShow = isShow;
        }
    }

    #region Quest Event:
    /// <summary>Raised when a scene's quest begins tracking.</summary>
    public struct QuestStartedEvent : IEvent
    {
        public Quest.QuestData Quest { get; }
        public QuestStartedEvent(Quest.QuestData quest) => Quest = quest;
    }

    /// <summary>Raised each time quest progress increments.</summary>
    public struct QuestProgressEvent : IEvent
    {
        public Quest.QuestData Quest { get; }
        public QuestProgressEvent(Quest.QuestData quest) => Quest = quest;
    }

    /// <summary>Raised when the quest target is met.</summary>
    public struct QuestCompletedEvent : IEvent
    {
        public Quest.QuestData Quest { get; }
        public QuestCompletedEvent(Quest.QuestData quest) => Quest = quest;
    }

    /// <summary>Raised when the player rescues (interacts with) a <see cref="NPC.RescuableNpc"/>.</summary>
    /// CAn share this event for all the object that have same behaviour collect something.
    public struct NpcRescuedEvent : IEvent { }

    /// <summary>Raised by any object that wants to grant the player skill points (chests, quest rewards, etc.).</summary>
    public struct SkillPointRewardEvent : IEvent
    {
        public float Amount { get; }
        public SkillPointRewardEvent(float amount) => Amount = amount;
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
}
