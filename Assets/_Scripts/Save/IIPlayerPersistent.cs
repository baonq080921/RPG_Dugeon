using System.Collections;
using SaveData;

/// <summary>
/// Implemented by the player component that knows how to read and restore its own persistent state.
/// Lives in Save/ (global namespace) so SaveManager can reference it without depending on player types.
/// </summary>
public interface IIPlayerPersistent
{
    /// <summary>Snapshots current player state into a serialisable DTO.</summary>
    public PlayerSaveData GetPlayerData();

    /// <summary>Restores player state from a previously snapshotted DTO.</summary>
    public IEnumerator RestoreFromSaveData(PlayerSaveData data, bool restorePosition);
}
