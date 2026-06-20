using System;

namespace Interfaces
{
    /// <summary>
    /// Implemented by any scene entity whose state should survive scene reloads (enemy killed, chest opened, item collected, etc.).
    /// Fire <see cref="OnPersisted"/> when the state changes; restore it in <see cref="RestoreState"/>.
    /// <see cref="scene.SceneEntityManager"/> handles subscription and restore automatically —
    /// no changes to SceneEntityManager are needed when adding a new persistable type.
    /// </summary>
    public interface IScenePersistable
    {
        string SceneEntityId { get; }

        /// <summary>Raised once when this entity's state changes and should be saved (e.g. enemy dies, chest opens).</summary>
        event Action OnPersisted;

        /// <summary>Puts this entity back into its already-used state on load, without re-triggering side effects (drops, animations, etc.).</summary>
        void RestoreState();
    }
}
