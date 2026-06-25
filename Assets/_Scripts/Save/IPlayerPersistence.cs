using System.Collections;

namespace Save
{
    /// <summary>
    /// Abstraction implemented by the player side (on the Player GameObject) that knows how to
    /// capture its own state into a <see cref="PlayerSaveData"/> and restore it back.
    /// Lets the Save layer persist the player without depending on the concrete player types.
    /// </summary>
    public interface IPlayerPersistence
    {
        /// <summary>Writes the current player-side state (health, exp, stats, inventory, skill tree) into <paramref name="data"/>.</summary>
        void Capture(PlayerSaveData data);

        /// <summary>Applies the player-side state from <paramref name="data"/> back onto the player.</summary>
        /// <param name="restorePosition">When true, also restores the saved world position (false for portal transitions where the spawn point handles position).</param>
        IEnumerator Restore(PlayerSaveData data, bool restorePosition);
    }
}
