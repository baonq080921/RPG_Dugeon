using System.Collections;
using System.Collections.Generic;
using System.IO;
using Base;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Save
{
    /// <summary>
    /// Coordinates the save file: disk I/O plus world/quest/scene state.
    /// Player-specific capture/restore is delegated to <see cref="IPlayerPersistence"/>
    /// (implemented on the Player object), so this layer no longer references the player namespace.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private static string SavePath =>
            Path.Combine(Application.persistentDataPath, "save.json");

        private PlayerSaveData _pendingLoad;
        private PlayerSaveData _transitionSnapshot;
        private readonly HashSet<string> _completedQuestScenes = new();
        private readonly Dictionary<string, SceneStateData> _sceneStates = new();
        private readonly Dictionary<string, int> _questProgress = new();

        /// <summary>True when a portal snapshot is waiting to be applied to the new scene's player.</summary>
        public bool HasTransitionSnapshot => _transitionSnapshot != null;

        private void Awake()
        {
            ServiceLocator.Register<SaveManager>(this);
        }

        // -------------------------------------------------------------------------
        // Save
        // -------------------------------------------------------------------------

        /// <summary>Writes current player + world state to disk.</summary>
        public void Save()
        {
            var persistence = ServiceLocator.Get<IPlayerPersistence>();
            if (persistence == null)
            {
                Debug.LogWarning("[SaveManager] Save called but no IPlayerPersistence is registered.");
                return;
            }

            var data = new PlayerSaveData();
            FillWorldState(data);
            persistence.Capture(data);
            WriteToDisk(data);
        }

        /// <summary>
        /// Called by <see cref="scene.SceneTransitionManager"/> before the old scene is unloaded.
        /// Captures state to memory so it can be restored in the new scene. Does NOT write to disk.
        /// </summary>
        public void SnapshotForTransition()
        {
            var persistence = ServiceLocator.Get<IPlayerPersistence>();
            if (persistence == null) return;

            var data = new PlayerSaveData();
            FillWorldState(data);
            persistence.Capture(data);
            _transitionSnapshot = data;
        }

        /// <summary>Fills the world/quest/scene portion of <paramref name="data"/> owned by this manager.</summary>
        private void FillWorldState(PlayerSaveData data)
        {
            data.sceneName = SceneManager.GetActiveScene().name;

            foreach (var sceneName in _completedQuestScenes)
                data.completedQuestScenes.Add(sceneName);

            foreach (var state in _sceneStates.Values)
                data.sceneStates.Add(state);

            foreach (var kvp in _questProgress)
                data.questProgress.Add(new QuestProgressEntry { sceneName = kvp.Key, progress = kvp.Value });
        }

        private void WriteToDisk(PlayerSaveData data)
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
            Debug.Log($"[SaveManager] Saved → {SavePath}");
        }

        // -------------------------------------------------------------------------
        // Restore after portal transition (no scene load, no position restore)
        // -------------------------------------------------------------------------

        /// <summary>
        /// Called by <see cref="scene.SceneTransitionManager"/> after the new scene finishes loading.
        /// Applies the in-memory snapshot — everything except position (handled by the spawn point).
        /// </summary>
        public IEnumerator RestoreAfterTransition()
        {
            if (_transitionSnapshot == null) yield break;

            var persistence = ServiceLocator.Get<IPlayerPersistence>();
            if (persistence == null) yield break;

            yield return StartCoroutine(persistence.Restore(_transitionSnapshot, restorePosition: false));
            ApplyWorldState(_transitionSnapshot);
            _transitionSnapshot = null;
            Debug.Log("[SaveManager] Transition restore complete.");
        }

        // -------------------------------------------------------------------------
        // Full disk load
        // -------------------------------------------------------------------------

        /// <summary>Returns true if a save file exists on disk.</summary>
        public bool HasSave() => File.Exists(SavePath);

        /// <summary>Marks a scene's quest as completed in the in-memory set. Call <see cref="Save"/> afterward to persist.</summary>
        public void MarkQuestComplete(string sceneName) => _completedQuestScenes.Add(sceneName);

        /// <summary>Returns true if the quest for <paramref name="sceneName"/> was already completed.</summary>
        public bool IsQuestComplete(string sceneName) => _completedQuestScenes.Contains(sceneName);

        /// <summary>Stores the latest numeric progress for the quest in <paramref name="sceneName"/>.</summary>
        public void SetQuestProgress(string sceneName, int progress)
        {
            DebugCustom.Log("Quest" + GetQuestProgress(sceneName) + "and progress is" + progress);
            _questProgress[sceneName] = progress;
        }

        /// <summary>Returns the saved progress for the quest in <paramref name="sceneName"/>, or 0 if none.</summary>
        public int GetQuestProgress(string sceneName)
            => _questProgress.TryGetValue(sceneName, out var p) ? p : 0;

        // -------------------------------------------------------------------------
        // Scene state (persisted to disk — restored on Continue, cleared on New Game)
        // -------------------------------------------------------------------------

        /// <summary>
        /// Clears all saved progress and loads <paramref name="startSceneName"/> fresh.
        /// Call this when the player chooses New Game from the main menu.
        /// </summary>
        public void StartNewGame(string startSceneName)
        {
            _sceneStates.Clear();
            _completedQuestScenes.Clear();
            _questProgress.Clear();
            if (File.Exists(SavePath))
                File.Delete(SavePath);
            SceneManager.LoadScene(startSceneName);
        }

        /// <summary>Records that the entity with <paramref name="entityId"/> has been permanently changed in <paramref name="sceneName"/> this session.</summary>
        public void MarkEntityPersisted(string sceneName, string entityId)
        {
            EnsureSceneState(sceneName).persistedEntityIds.Add(entityId);
        }

        /// <summary>Returns the state data for <paramref name="sceneName"/>, or null if no entities have been interacted with yet.</summary>
        public SceneStateData GetSceneState(string sceneName)
            => _sceneStates.TryGetValue(sceneName, out var data) ? data : null;

        private SceneStateData EnsureSceneState(string sceneName)
        {
            if (!_sceneStates.TryGetValue(sceneName, out var data))
            {
                data = new SceneStateData { sceneName = sceneName };
                _sceneStates[sceneName] = data;
            }
            return data;
        }

        /// <summary>Reads the save file, loads the stored scene, then restores full player state including position.</summary>
        public void Load()
        {
            if (!HasSave())
            {
                Debug.LogWarning("[SaveManager] No save file found.");
                return;
            }

            _pendingLoad = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(SavePath));

            // Pre-populate quest data before the scene loads so SceneQuestController.Start()
            // reads the correct saved values instead of 0.
            _completedQuestScenes.Clear();
            foreach (var s in _pendingLoad.completedQuestScenes)
                _completedQuestScenes.Add(s);

            _questProgress.Clear();
            foreach (var entry in _pendingLoad.questProgress)
                if (!string.IsNullOrEmpty(entry.sceneName))
                    _questProgress[entry.sceneName] = entry.progress;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(_pendingLoad.sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine(ApplyAfterFullLoad());
        }

        private IEnumerator ApplyAfterFullLoad()
        {
            yield return null; // wait for Awake/Start in new scene

            var persistence = ServiceLocator.Get<IPlayerPersistence>();
            if (persistence == null)
            {
                Debug.LogError("[SaveManager] Load failed: IPlayerPersistence not found after scene load.");
                yield break;
            }

            yield return StartCoroutine(persistence.Restore(_pendingLoad, restorePosition: true));
            ApplyWorldState(_pendingLoad);
            _pendingLoad = null;
            Debug.Log("[SaveManager] Full load complete.");
        }

        /// <summary>Applies the world/quest/scene portion of <paramref name="data"/> after the player has been restored.</summary>
        private void ApplyWorldState(PlayerSaveData data)
        {
            _completedQuestScenes.Clear();
            foreach (var sceneName in data.completedQuestScenes)
                _completedQuestScenes.Add(sceneName);

            _sceneStates.Clear();
            foreach (var state in data.sceneStates)
                if (!string.IsNullOrEmpty(state.sceneName))
                    _sceneStates[state.sceneName] = state;

            _questProgress.Clear();
            foreach (var entry in data.questProgress)
                if (!string.IsNullOrEmpty(entry.sceneName))
                    _questProgress[entry.sceneName] = entry.progress;

            // SceneEntityManager.Start() ran before _sceneStates was restored. Notify scenes to
            // re-apply now — via an event so this layer doesn't reference the scene package.
            EventBus<SceneStateRestoredEvent>.Raise(new SceneStateRestoredEvent());
        }
    }
}
