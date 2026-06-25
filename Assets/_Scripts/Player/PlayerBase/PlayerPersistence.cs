using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Save;
using UnityEngine;

namespace player
{
    /// <summary>
    /// Owns serialization of all player-side state (position, health, exp, stats, inventory,
    /// equipment, skill tree) behind <see cref="IPlayerPersistence"/>. Lives on the Player
    /// GameObject so the Save layer can persist the player without referencing player types.
    /// </summary>
    public class PlayerPersistence : MonoBehaviour, IPlayerPersistence
    {
        private Player _player;
        private ItemDataRegistry _registry;

        private void Awake()
        {
            _player = GetComponent<Player>();

            _registry = Resources.Load<ItemDataRegistry>("ItemDataRegistry");
            if (_registry == null)
                Debug.LogError("[PlayerPersistence] ItemDataRegistry not found in Resources.");
            else
                _registry.Initialize();

            ServiceLocator.Register<IPlayerPersistence>(this);
        }

        /// <inheritdoc/>
        public void Capture(PlayerSaveData data)
        {
            var inventory   = _player.GetComponent<PlayerInventory>();
            var playerLevel = _player.GetComponent<PlayerLevel>();
            var playerStat  = _player.GetComponent<EntityStat>();

            data.posX           = _player.transform.position.x;
            data.posY           = _player.transform.position.y;
            data.currentHealth  = _player.playerHealth.CurrentHealth;
            data.currentExp     = playerLevel != null ? playerLevel.CurrentExp     : 0f;
            data.currentLevel   = playerLevel != null ? playerLevel.Level          : 1;
            data.expToNextLevel = playerLevel != null ? playerLevel.ExpToNextLevel : 0f;
            data.strengthPoints     = playerStat != null ? playerStat.GetAllocatedPoints(StatType.Strength)     : 0;
            data.agilityPoints      = playerStat != null ? playerStat.GetAllocatedPoints(StatType.Agility)      : 0;
            data.intelligencePoints = playerStat != null ? playerStat.GetAllocatedPoints(StatType.Intelligence) : 0;
            data.vitalityPoints     = playerStat != null ? playerStat.GetAllocatedPoints(StatType.Vitality)     : 0;
            data.currentGolds       = inventory  != null ? inventory.Money : 0;

            if (inventory != null)
                GetInventoryData(inventory, data);

            var skillTree = ServiceLocator.Get<UIManager>()?.uISkillTree;
            if (skillTree != null)
                GetSkillTreeData(inventory, data, skillTree);
        }

        /// <inheritdoc/>
        public IEnumerator Restore(PlayerSaveData data, bool restorePosition)
        {
            var inventory = _player.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                Debug.LogError("[PlayerPersistence] Restore failed: PlayerInventory not found.");
                yield break;
            }

            if (restorePosition)
                _player.transform.position = new Vector2(data.posX, data.posY);

            _player.playerHealth.RestoreHealth(data.currentHealth);
            _player.playerLevel.RestoreExp(data);

            inventory.ClearAllInventory();
            inventory.SetMoney(data.currentGolds);

            foreach (var entry in data.inventory)
            {
                if (string.IsNullOrEmpty(entry.itemId)) continue;
                var itemData = _registry?.Get(entry.itemId);
                if (itemData == null)
                {
                    Debug.LogWarning($"[PlayerPersistence] Unknown itemId '{entry.itemId}' — skipped.");
                    continue;
                }
                var item = new ItemInventory(itemData);
                inventory.itemInventoriesList.Add(item);
                for (int i = 1; i < entry.stackSize; i++)
                    item.AddStack();
            }

            var playerStat = _player.GetComponent<EntityStat>();
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
                    Debug.LogWarning($"[PlayerPersistence] Unknown equipped itemId '{entry.itemId}' — skipped.");
                    continue;
                }
                if (!Enum.TryParse(entry.slotType, out EquipSlotType slotType)) continue;
                var slot = inventory.equipList.Find(s => s.slotType == slotType);
                if (slot == null) continue;

                var item = new ItemInventory(itemData);
                slot.equipItem = item;
                item.AddModifiers(playerStat);
                item.AddItemEffect(_player);
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
                        Debug.LogWarning($"[PlayerPersistence] Unknown skill tree node '{entry.nodeKey}' — skipped.");
                }

                foreach (var handler in uiSkillTree.GetComponentsInChildren<UIConnectedHandler>())
                    handler.RefreshLineColors();
            }

            yield return null;
        }

        private static void GetSkillTreeData(PlayerInventory inventory, PlayerSaveData data, UISkillTree skillTree)
        {
            data.skillTree.skillPoints = inventory?.SkillPoints ?? 0f;
            foreach (var node in skillTree.GetComponentsInChildren<UITreeNode>(true))
            {
                if (node.skillTreeData == null || !node.isUnlocked) continue;
                data.skillTree.nodes.Add(new NodeSaveEntry { nodeKey = node.skillTreeData.name });
            }
        }

        private static void GetInventoryData(PlayerInventory inventory, PlayerSaveData data)
        {
            foreach (var item in inventory.itemInventoriesList)
            {
                if (item?.itemData == null) continue;
                data.inventory.Add(new ItemSaveEntry
                {
                    itemId = item.itemData.ItemId,
                    stackSize = item.stackSize,
                });
            }
            foreach (var slot in inventory.equipList)
            {
                data.equipment.Add(new EquipSaveEntry
                {
                    slotType = slot.slotType.ToString(),
                    itemId = slot.HasItem() ? slot.equipItem.itemData.ItemId : string.Empty,
                });
            }
        }
    }
}
