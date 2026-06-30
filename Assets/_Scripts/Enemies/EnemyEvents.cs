using Base;

namespace enemy
{
    /// <summary>
    /// Raised by an enemy when it dies, carrying the enemy instance so a boss can trigger its last-attempt attack.
    /// </summary>
    public struct EnemyDiedForBossEvent : IEvent
    {
        public Enemy enemy { get; }
        public EnemyDiedForBossEvent(Enemy enemy)
        {
            this.enemy = enemy;
        }
    }
}
