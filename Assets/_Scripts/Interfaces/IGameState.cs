namespace Interfaces
{
    /// <summary>
    /// Contract for the global game lifecycle / pause state.
    /// Decouples callers (player, UI) from the concrete <c>GameManager</c> so lower layers
    /// never reference the System package directly.
    /// </summary>
    public interface IGameState
    {
        /// <summary>True while the game is paused (time frozen).</summary>
        bool IsPause { get; }

        /// <summary>Sets the pause state and freezes/unfreezes time.</summary>
        /// <param name="pause">True to pause, false to resume.</param>
        void SetPause(bool pause);
    }
}
