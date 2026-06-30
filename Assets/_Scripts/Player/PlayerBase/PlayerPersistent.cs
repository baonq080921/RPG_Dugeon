using System;
using System.Collections;
using Base;
using player;
using Save;
using SaveData;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistent : MonoBehaviour, IIPlayerPersistent
{
    private Player _player;
    private ItemDataRegistry _registry;

    private void Awake()
    {
        _player   = GetComponent<Player>();
        _registry = Resources.Load<ItemDataRegistry>("ItemDataRegistry");
        if (_registry == null)
            Debug.LogError("[PlayerPersistent] ItemDataRegistry not found in Resources.");
        else
            _registry.Initialize();

        ServiceLocator.Register<IIPlayerPersistent>(this);
    }

    // -------------------------------------------------------------------------
    // Save
    // -------------------------------------------------------------------------

    /// <inheritdoc/>
    public PlayerSaveData GetPlayerData()
    {
        var playerLevel     = _player.playerLevel;
        var playerInventory = _player.playerInventory;

        var data = new PlayerSaveData
        {
            sceneName          = SceneManager.GetActiveScene().name,
            posX               = _player.transform.position.x,
            posY               = _player.transform.position.y,
            currentHealth      = _player.entityHealth.CurrentHealth,
            currentExp         = playerLevel     != null ? playerLevel.CurrentExp        : 0f,
            currentLevel       = playerLevel     != null ? playerLevel.Level             : 1,
            expToNextLevel     = playerLevel     != null ? playerLevel.ExpToNextLevel    : 0f,
            strengthPoints     = _player.entityStat != null ? _player.entityStat.GetAllocatedPoints(StatType.Strength)     : 0,
            agilityPoints      = _player.entityStat != null ? _player.entityStat.GetAllocatedPoints(StatType.Agility)      : 0,
            intelligencePoints = _player.entityStat != null ? _player.entityStat.GetAllocatedPoints(StatType.Intelligence) : 0,
            vitalityPoints     = _player.entityStat != null ? _player.entityStat.GetAllocatedPoints(StatType.Vitality)     : 0,
            currentGolds       = playerInventory != null ? playerInventory.Money        : 0,
            skillPoints        = playerInventory != null ? playerInventory.SkillPoints  : 0,
        };

        if (playerInventory != null)
            GetInventoryData(playerInventory, data);

        var treeNodePersistent = ServiceLocator.Get<ITreeNodePersistent>();
        if (treeNodePersistent != null)
            GetSkillTreeData(playerInventory, data, treeNodePersistent);

        return data;
    }

    private void GetInventoryData(PlayerInventory inventory, PlayerSaveData data)
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

    private void GetSkillTreeData(PlayerInventory inventory, PlayerSaveData data, ITreeNodePersistent treeNodePersistent)
    {
        foreach (var node in treeNodePersistent.GetUINodesInSkillTree())
        {
            if (node.skillTreeData == null || !node.isUnlocked) continue;
            data.skillTree.nodes.Add(new NodeSaveEntry { nodeKey = node.skillTreeData.name });
        }
    }

    // -------------------------------------------------------------------------
    // Restore
    // -------------------------------------------------------------------------

    /// <inheritdoc/>
    public IEnumerator RestoreFromSaveData(PlayerSaveData data, bool restorePosition)
    {
        if (restorePosition)
            transform.position = new Vector2(data.posX, data.posY);

        _player.playerHealth.RestoreHealth(data.currentHealth);
        _player.playerLevel.RestoreExp(data.currentExp, data.currentLevel, data.expToNextLevel);

        var inventory = _player.playerInventory;
        inventory.ClearAllInventory();
        inventory.SetMoney(data.currentGolds);

        foreach (var entry in data.inventory)
        {
            if (string.IsNullOrEmpty(entry.itemId)) continue;
            var itemData = _registry?.Get(entry.itemId);
            if (itemData == null)
            {
                Debug.LogWarning($"[PlayerPersistent] Unknown itemId '{entry.itemId}' — skipped.");
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
                Debug.LogWarning($"[PlayerPersistent] Unknown equipped itemId '{entry.itemId}' — skipped.");
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

        yield return null;
    }
}
