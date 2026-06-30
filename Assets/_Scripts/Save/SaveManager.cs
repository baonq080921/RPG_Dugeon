using System.Collections;
using System.Collections.Generic;
using System.IO;
using Base;
using Interfaces;
using SaveData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Save
{
    /// <summary>
    /// Handles writing and reading the player save file.
    /// Also holds an in-memory <see cref="_transitionSnapshot"/> so the
    /// <see cref="scene.SceneTransitionManager"/> can restore player state after a
    /// portal scene load without needing to touch disk again.
    /// Player-specific restoration is delegated to <see cref="IIPlayerPersistent"/> so
    /// this layer never references player types directly.
    /// </summary>
    public class SaveManager : MonoBehaviour, ISaveService
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
            ServiceLocator.Register<ISaveService>(this);
        }

        // -------------------------------------------------------------------------
        // Save
        // -------------------------------------------------------------------------

        /// <summary>Writes current player state to disk.</summary>
        public void Save()
        {
            var playerPersistent = ServiceLocator.Get<IIPlayerPersistent>();
            if (playerPersistent == null)
            {
                Debug.LogWarning("[SaveManager] Save called but IIPlayerPersistent not registered.");
                return;
            }
            WriteToDisk(BuildSaveData(playerPersistent));
        }

        /// <summary>
        /// Called by <see cref="scene.SceneTransitionManager"/> before the old scene is unloaded.
        /// Captures player state to memory so it can be restored in the new scene.
        /// Does NOT write to disk — save only happens at checkpoints or manual saves.
        /// </summary>
        public void SnapshotForTransition(IIPlayerPersistent iPlayerPersistent)
        {
            _transitionSnapshot = BuildSaveData(iPlayerPersistent);
        }

        private PlayerSaveData BuildSaveData(IIPlayerPersistent iPlayerPersistent)
        {
            var playerCurrentData = iPlayerPersistent.GetPlayerData();

            foreach (var sceneName in _completedQuestScenes)
                playerCurrentData.completedQuestScenes.Add(sceneName);

            foreach (var state in _sceneStates.Values)
                playerCurrentData.sceneStates.Add(state);

            foreach (var kvp in _questProgress)
                playerCurrentData.questProgress.Add(new QuestProgressEntry { sceneName = kvp.Key, progress = kvp.Value });

            return playerCurrentData;
        }

        private void WriteToDisk(PlayerSaveData data)
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
            Debug.Log($"[SaveManager] Saved → {SavePath}");
        }

        // -------------------------------------------------------------------------
        // Restore after portal transition
        // -------------------------------------------------------------------------

        /// <summary>
        /// Called by <see cref="scene.SceneTransitionManager"/> after the new scene finishes loading.
        /// Applies the in-memory snapshot — everything except position (handled by the spawn point).
        /// </summary>
        public IEnumerator RestoreAfterTransition()
        {
            if (_transitionSnapshot == null) yield break;
            yield return StartCoroutine(ApplyData(_transitionSnapshot, restorePosition: false));
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
        // Scene state
        // -------------------------------------------------------------------------

        /// <summary>
        /// Clears all saved progress and loads <paramref name="startSceneName"/> fresh.
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

        /// <summary>Records that the entity with <paramref name="entityId"/> has been permanently changed in <paramref name="sceneName"/>.</summary>
        public void MarkEntityPersisted(string sceneName, string entityId)
        {
            EnsureSceneState(sceneName).persistedEntityIds.Add(entityId);
        }

        /// <summary>Returns the state data for <paramref name="sceneName"/>, or null if no entities have been interacted with yet.</summary>
        public SceneStateData GetSceneState(string sceneName)
            => _sceneStates.TryGetValue(sceneName, out var data) ? data : null;

        /// <inheritdoc/>
        public bool IsEntityPersisted(string sceneName, string entityId)
            => _sceneStates.TryGetValue(sceneName, out var state) && state.persistedEntityIds.Contains(entityId);

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

            // Deserialization can throw on a corrupted/partially-written save file
            // (power loss while saving, disk error). Guard it so a bad file is
            // treated as "no save" instead of crashing the game.
            try
            {
                _pendingLoad = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(SavePath));
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[SaveManager] Failed to parse save file, treating as no save: {e.Message}");
                return;
            }

            if (_pendingLoad == null)
            {
                Debug.LogWarning("[SaveManager] Save file deserialized to null; aborting load.");
                return;
            }

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
            yield return StartCoroutine(ApplyData(_pendingLoad, restorePosition: true));
            _pendingLoad = null;
            Debug.Log("[SaveManager] Full load complete.");
        }

        // -------------------------------------------------------------------------
        // Shared restore logic
        // -------------------------------------------------------------------------

        private IEnumerator ApplyData(PlayerSaveData data, bool restorePosition)
        {
            var playerPersistent = ServiceLocator.Get<IIPlayerPersistent>();
            if (playerPersistent == null)
            {
                Debug.LogError("[SaveManager] IIPlayerPersistent not found — restore aborted.");
                yield break;
            }

            yield return StartCoroutine(playerPersistent.RestoreFromSaveData(data, restorePosition));

            EventBus<SkillTreeRestoreEvent>.Raise(new SkillTreeRestoreEvent(data.skillTree, data.skillPoints));

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

            // SceneEntityManager.Start() ran before _sceneStates was restored; notify it to re-apply now.
            EventBus<SceneStateRestoredEvent>.Raise(new SceneStateRestoredEvent());
        }
    }
}
