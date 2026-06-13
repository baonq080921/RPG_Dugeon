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
    /// Registered with <see cref="ServiceLocator"/> by <see cref="GameBootstrapper"/> at startup.
    /// Save path: <see cref="Application.persistentDataPath"/>/save.json
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private static string SavePath =>
            Path.Combine(Application.persistentDataPath, "save.json");

        private ItemDataRegistry _registry;
        private PlayerSaveData   _pendingLoad;

        private void Awake()
        {
            _registry = Resources.Load<ItemDataRegistry>("ItemDataRegistry");
            if (_registry == null)
                Debug.LogError("[SaveManager] ItemDataRegistry not found in Resources. Create it via RPG/Save/ItemDataRegistry.");
            else
                _registry.Initialize();

            ServiceLocator.Register<SaveManager>(this);
        }

        // -------------------------------------------------------------------------
        // Save
        // -------------------------------------------------------------------------

        /// <summary>Serialises the current player state to disk.</summary>
        public void Save()
        {
            var player    = FindObjectOfType<Player>();
            var inventory = player != null ? player.GetComponent<PlayerInventory>() : null;
            var playerLevel = player != null ? player.GetComponent<PlayerLevel>(): null;

            if (player == null || inventory == null || playerLevel == null)
            {
                Debug.LogWarning("[SaveManager] Save called but Player / PlayerInventory not found in scene.");
                return;
            }

            var data = new PlayerSaveData
            {
                sceneName     = SceneManager.GetActiveScene().name,
                posX          = player.transform.position.x,
                posY          = player.transform.position.y,
                currentHealth = player.playerHealth.CurrentHealth,
                currentExp = player.playerLevel.CurrentExp,
                currentLevel = player.playerLevel.Level,
                expToNextLevel = player.playerLevel.ExpToNextLevel,
                
            };

            // Bag items
            foreach (var item in inventory.itemInventoriesList)
            {
                if (item?.itemData == null) continue;
                data.inventory.Add(new ItemSaveEntry
                {
                    itemId    = item.itemData.ItemId,
                    stackSize = item.stackSize,
                });
            }

            // Equipped items
            foreach (var slot in inventory.equipList)
            {
                data.equipment.Add(new EquipSaveEntry
                {
                    slotType = slot.slotType.ToString(),
                    itemId   = slot.HasItem() ? slot.equipItem.itemData.ItemId : string.Empty,
                });
            }

            // Skill tree
            var skillTree = ServiceLocator.Get<UIManager>()?.uISkillTree;
            if (skillTree != null)
            {
                data.skillTree.skillPoints = skillTree.SkillPoints;
                //Save all the node that is open in the SKill Tree
                //if node is not open we simply not care about it
                foreach (var node in skillTree.GetComponentsInChildren<UITreeNode>())
                {
                    if (node.skillTreeData == null || !node.isUnlocked) continue;
                    data.skillTree.nodes.Add(new NodeSaveEntry { nodeKey = node.skillTreeData.SkillTreeId });
                }
            }

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
            Debug.Log($"[SaveManager] Saved to {SavePath}");
        }

        // -------------------------------------------------------------------------
        // Load
        // -------------------------------------------------------------------------

        /// <summary>Returns true if a save file exists on disk.</summary>
        public bool HasSave() => File.Exists(SavePath);

        /// <summary>Reads the save file, loads the stored scene, then restores player state.</summary>
        public void Load()
        {
            if (!HasSave())
            {
                Debug.LogWarning("[SaveManager] No save file found.");
                return;
            }

            _pendingLoad = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(SavePath));
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(_pendingLoad.sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine(ApplyLoadedData(_pendingLoad));
            _pendingLoad = null;
        }

        private IEnumerator ApplyLoadedData(PlayerSaveData data)
        {
            // Wait one frame so all Awake/Start calls in the new scene complete.
            yield return null;

            var player    = FindObjectOfType<Player>();
            var inventory = player != null ? player.GetComponent<PlayerInventory>() : null;

            if (player == null || inventory == null)
            {
                Debug.LogError("[SaveManager] Load failed: Player / PlayerInventory not found after scene load.");
                yield break;
            }

            // Position & health && Exp 
            player.transform.position = new Vector2(data.posX, data.posY);
            player.playerHealth.RestoreHealth(data.currentHealth);
            player.playerLevel.RestoreExp(data);

            
            // Clear existing inventory
            inventory.ClearAllInventory();

            // Restore bag items (add directly to bypass the size guard during a load)
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

            // Restore equipment slots
            var playerStat = player.GetComponent<EntityStat>();
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

            // Skill tree
            var uiSkillTree = ServiceLocator.Get<UIManager>()?.uISkillTree;
            if (uiSkillTree != null)
            {
                var allNodes = uiSkillTree.GetComponentsInChildren<UITreeNode>();

                foreach (var node in allNodes)
                    node.ResetNode();

                uiSkillTree.RestoreSkillPoints(data.skillTree.skillPoints);

                var nodeMap = new Dictionary<string, UITreeNode>();
                foreach (var node in allNodes)
                    if (node.skillTreeData != null && !string.IsNullOrEmpty(node.skillTreeData.SkillTreeId))
                        nodeMap[node.skillTreeData.SkillTreeId] = node;

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

            Debug.Log("[SaveManager] Load complete.");
        }
    }
}
