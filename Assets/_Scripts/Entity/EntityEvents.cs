using Base;

namespace entity
{
    /// <summary>
    /// Raised when the player gains enough XP to level up. Carries the stat sheet so the UI can apply the chosen bonus.
    /// Lives in the entity package (not Base) because its payload is an <see cref="EntityStat"/>, keeping the Base
    /// layer free of any dependency on the entity package.
    /// </summary>
    public struct PlayerLevelUpEvent : IEvent
    {
        public EntityStat PlayerStat { get; }
        public PlayerLevelUpEvent(EntityStat playerStat) => PlayerStat = playerStat;
    }
}
