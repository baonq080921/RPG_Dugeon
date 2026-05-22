namespace Base
{
    /// <summary>Raised when the player's health reaches zero and the death state is entered.</summary>
    public struct PlayerDiedEvent : IEvent { }

    /// <summary>Raised when an enemy's health reaches zero.</summary>
    public struct EnemyDiedEvent : IEvent { }
}
