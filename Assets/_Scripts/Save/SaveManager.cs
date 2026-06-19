using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Base;
using player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Save
{
    /// <summary>
    /// Handles writing and reading the player save file.
    /// Also holds an in-memory <see cref="_transitionSnapshot"/> so the
    /// <see cref="scene.SceneTransitionManager"/> can restore player state after a
    /// portal scene load without needing to touch disk again.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private static string SavePath =>
            Path.Combine(Application.persistentDataPath, "save.json");

        private ItemDataRegistry _registry;
        private PlayerSaveData   _pendingLoad;
        private PlayerSaveData   _transitionSnapshot;
        private readonly HashSet<string> _completedQuestScenes = new();
        private readonly Dictionary<string, SceneStateData> _sceneStates = new();
        private readonly Dictionary<string, int> _questProgress = new();

        /// <summary>True when a portal snapshot is waiting to be applied to the new scene's player.</summary>
        public bool HasTransitionSnapshot => _transitionSnapshot != null;

        private void Awake()
        {
            _registry = Resources.Load<ItemDataRegistry>("ItemDataRegistry");
            if (_registry == null)
                Debug.LogError("[SaveManager] ItemDataRegistry not found in Resources.");
            else
                _registry.Initialize();

            ServiceLocator.Register<SaveManager>(this);
        }

        // -------------------------------------------------------------------------
        // Save
        // -------------------------------------------------------------------------

        /// <summary>Writes current player state to disk.</summary>
        public void Save()
        {
            var player = FindObjectOfType<Player>();
            if (player == null)
            {
                Debug.LogWarning("[SaveManager] Save called but Player not found in scene.");
                return;
            }
            WriteToDisk(BuildSaveData(player));
        }

        /// <summary>
        /// Called by <see cref="scene.SceneTransitionManager"/> before the old scene is unloaded.
        /// Captures player state to memory so it can be restored in the new scene.
        /// Does NOT write to disk — save only happens at checkpoints or manual saves.
        /// </summary>
        public void SnapshotForTransition(Player player)
        {
            _transitionSnapshot = BuildSaveData(player);
        }

        private PlayerSaveData BuildSaveData(Player player)
        {
            var inventory   = player.GetComponent<PlayerInventory>();
            var playerLevel = player.GetComponent<PlayerLevel>();

            var playerStat = player.GetComponent<EntityStat>();
            var data = new PlayerSaveData
            {
                sceneName         = SceneManager.GetActiveScene().name,
                posX              = player.transform.position.x,
                posY              = player.transform.position.y,
                currentHealth     = player.playerHealth.CurrentHealth,
                currentExp        = playerLevel != null ? playerLevel.CurrentExp     : 0f,
                currentLevel      = playerLevel != null ? playerLevel.Level          : 1,
                expToNextLevel    = playerLevel != null ? playerLevel.ExpToNextLevel : 0f,
                strengthPoints     = playerStat != null ? playerStat.GetAllocatedPoints(StatType.Strength)     : 0,
                agilityPoints      = playerStat != null ? playerStat.GetAllocatedPoints(StatType.Agility)      : 0,
                intelligencePoints = playerStat != null ? playerStat.GetAllocatedPoints(StatType.Intelligence) : 0,
                vitalityPoints     = playerStat != null ? playerStat.GetAllocatedPoints(StatType.Vitality)     : 0,
            };

            if (inventory != null)
            {
                foreach (var item in inventory.itemInventoriesList)
                {
                    if (item?.itemData == null) continue;
                    data.inventory.Add(new ItemSaveEntry
                    {
                        itemId    = item.itemData.ItemId,
                        stackSize = item.stackSize,
                    });
                }
                foreach (var slot in inventory.equipList)
                {
                    data.equipment.Add(new EquipSaveEntry
                    {
                        slotType = slot.slotType.ToString(),
                        itemId   = slot.HasItem() ? slot.equipItem.itemData.ItemId : string.Empty,
                    });
                }
            }

            var skillTree = ServiceLocator.Get<UIManager>()?.uISkillTree;
            if (skillTree != null)
            {
                data.skillTree.skillPoints = inventory?.SkillPoints ?? 0f;
                foreach (var node in skillTree.GetComponentsInChildren<UITreeNode>(true))
                {
                    if (node.skillTreeData == null || !node.isUnlocked) continue;
                    data.skillTree.nodes.Add(new NodeSaveEntry { nodeKey = node.skillTreeData.name });
                }
            }

            foreach (var sceneName in _completedQuestScenes)
                data.completedQuestScenes.Add(sceneName);

            foreach (var state in _sceneStates.Values)
                data.sceneStates.Add(state);

            foreach (var kvp in _questProgress)
                data.questProgress.Add(new QuestProgressEntry { sceneName = kvp.Key, progress = kvp.Value });

            return data;
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
        /// Applies the in-memory snapshot to <paramref name="player"/> — everything except position
        /// (position is handled by the spawn point).
        /// </summary>
        public IEnumerator RestoreAfterTransition(Player player)
        {
            if (_transitionSnapshot == null) yield break;
            var inventory = player.GetComponent<PlayerInventory>();
            if (inventory == null) yield break;

            yield return StartCoroutine(ApplyData(_transitionSnapshot, player, inventory, restorePosition: false));
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

        /// <summary>Records that <paramref name="enemyId"/> was killed in <paramref name="sceneName"/> this session.</summary>
        public void MarkEnemyKilled(string sceneName, string enemyId)
        {
            EnsureSceneState(sceneName).killedEnemyIds.Add(enemyId);
        }

        /// <summary>Records that <paramref name="chestId"/> was opened in <paramref name="sceneName"/> this session.</summary>
        public void MarkChestOpened(string sceneName, string chestId)
        {
            EnsureSceneState(sceneName).openedChestIds.Add(chestId);
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

            var player    = FindObjectOfType<Player>();
            var inventory = player?.GetComponent<PlayerInventory>();
            if (player == null || inventory == null)
            {
                Debug.LogError("[SaveManager] Load failed: Player / PlayerInventory not found after scene load.");
                yield break;
            }

            yield return StartCoroutine(ApplyData(_pendingLoad, player, inventory, restorePosition: true));
            _pendingLoad = null;
            Debug.Log("[SaveManager] Full load complete.");
        }

        // -------------------------------------------------------------------------
        // Shared restore logic
        // -------------------------------------------------------------------------

        private IEnumerator ApplyData(PlayerSaveData data, Player player, PlayerInventory inventory, bool restorePosition)
        {
            if (restorePosition)
                player.transform.position = new Vector2(data.posX, data.posY);

            player.playerHealth.RestoreHealth(data.currentHealth);
            player.playerLevel.RestoreExp(data);

            inventory.ClearAllInventory();

            foreach (var entry in data.inventory)
            {
                if (string.IsNullOrEmpty(entry.itemId)) continue;
                var itemData = _registry?.Get(entry.itemId);
                if (itemData == null)
                {
                    Debug.LogWarning($"[SaveManager] Unknown itemId '{entry.itemId}' — skipped.");
                    continue;
                }
                var item = new ItemInventory(itemData);
                inventory.itemInventoriesList.Add(item);
                for (int i = 1; i < entry.stackSize; i++)
                    item.AddStack();
            }

            var playerStat = player.GetComponent<EntityStat>();
            playerStat.ResetAllStats();
            for (int i = 0; i < data.strengthPoints;     i++) playerStat.AddMajorStatPoint(StatType.Strength);
            for (int i = 0; i < data.agilityPoints;      i++) playerStat.AddMajorStatPoint(StatType.Agility);
            for (int i = 0; i < data.intelligencePoints; i++) playerStat.AddMajorStatPoint(StatType.Intelligence);
            for (int i = 0; i < data.vitalityPoints;     i++) playerStat.AddMajorStatPoint(StatType.Vitality);
            foreach (var entry in data.equipment)
            {
                if (string.IsNullOrEmpty(entry.itemId)) continue;
                var itemData = _registry?.Get(entry.itemId);
                if (itemData == null)
                {
                    Debug.LogWarning($"[SaveManager] Unknown equipped itemId '{entry.itemId}' — skipped.");
                    continue;
                }
                if (!Enum.TryParse(entry.slotType, out EquipSlotType slotType)) continue;
                var slot = inventory.equipList.Find(s => s.slotType == slotType);
                if (slot == null) continue;

                var item = new ItemInventory(itemData);
                slot.equipItem = item;
                item.AddModifiers(playerStat);
                item.AddItemEffect(player);
            }

            EventBus<OnInventoryChangedEvent>.Raise(new OnInventoryChangedEvent());

            var uiSkillTree = ServiceLocator.Get<UIManager>()?.uISkillTree;
            if (uiSkillTree != null)
            {
                var allNodes = uiSkillTree.GetComponentsInChildren<UITreeNode>(true);
                foreach (var node in allNodes)
                    node.ResetNode();

                inventory.SetSkillPoints(data.skillTree.skillPoints);

                var nodeMap = new Dictionary<string, UITreeNode>();
                foreach (var node in allNodes)
                    if (node.skillTreeData != null)
                        nodeMap[node.skillTreeData.name] = node;

                foreach (var entry in data.skillTree.nodes)
                {
                    if (nodeMap.TryGetValue(entry.nodeKey, out var node))
                        node.RestoreUnlocked();
                    else
                        Debug.LogWarning($"[SaveManager] Unknown skill tree node '{entry.nodeKey}' — skipped.");
                }

                foreach (var handler in uiSkillTree.GetComponentsInChildren<UIConnectedHandler>())
                    handler.RefreshLineColors();
            }

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
                {
                    _questProgress[entry.sceneName] = entry.progress;
                    Debug.Log("Scene Name"+ entry.sceneName + "progress "+ entry.progress);
                }

            // SceneEntityManager.Start() ran before _sceneStates was restored, so apply now.
            FindObjectOfType<scene.SceneEntityManager>()?.ApplySceneState();

            yield return null;
        }
    }
}
