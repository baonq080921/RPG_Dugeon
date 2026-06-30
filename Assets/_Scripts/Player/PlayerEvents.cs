using Base;

namespace player
{
    /// <summary>
    /// Raised when the player gains enough XP to level up.
    /// Carries the stat sheet so the UI can apply the chosen bonus.
    /// </summary>
    public struct PlayerLevelUpEvent : IEvent
    {
        public EntityStat PlayerStat { get; }
        public PlayerLevelUpEvent(EntityStat playerStat) => PlayerStat = playerStat;
    }

    public struct PlayerLastAttackEvent:IEvent{}
}
