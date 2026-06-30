using System.Collections;

namespace Interfaces
  {
      /// <summary>
      /// Persistence operations exposed to higher layers (UI, Quest, checkpoints, scene transitions)
      /// so they depend on this abstraction instead of the concrete SaveManager.
      /// </summary>
      public interface ISaveService
      {
          void Save();
          void Load();
          bool HasSave();
          void StartNewGame(string startSceneName);
          bool IsQuestComplete(string sceneName);
          int GetQuestProgress(string sceneName);
          void SetQuestProgress(string sceneName, int progress);
          void MarkQuestComplete(string sceneName);
          void MarkEntityPersisted(string sceneName, string entityId);
          bool IsEntityPersisted(string sceneName, string entityId);
          void SnapshotForTransition(IIPlayerPersistent playerPersistent);
          bool HasTransitionSnapshot { get; }
          IEnumerator RestoreAfterTransition();
      }
  }