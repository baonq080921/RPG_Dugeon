using System;
using player;
using UnityEngine;

public class UISkillTree : MonoBehaviour
{
    public static event Action OnReset;

    private PlayerInventory _playerInventory;

    public float SkillPoints => _playerInventory?.SkillPoints ?? 0f;

    private void OnEnable() => Player.ActivePlayerChanged += OnPlayerChanged;
    private void OnDisable() => Player.ActivePlayerChanged -= OnPlayerChanged;

    private void OnPlayerChanged(Player player) => _playerInventory = player.playerInventory;

    public bool EnoughSkillPoint(float cost) => _playerInventory?.CanSpendSkillPoints(cost) ?? false;

    public void ReduceSkillPoint(float cost) => _playerInventory?.SpendSkillPoints(cost);

    public void RefundSkillPoint(float cost) => _playerInventory?.AddSkillPoints(cost);

    public void RestoreSkillPoints(float amount) => _playerInventory?.SetSkillPoints(amount);

    [ContextMenu("Arrange Skill Tree Nodes")]
    public void ArrangeSkillTreeNodes()
    {
        foreach (var handler in GetComponentsInChildren<UIConnectedHandler>())
            handler.ArrangeChildNodes();
    }

    [ContextMenu("Reset Skill Tree")]
    public void ResetSkillTree()
    {
        foreach (var node in GetComponentsInChildren<UITreeNode>(true))
        {
            if (node.isUnlocked && node.skillTreeData != null)
                RefundSkillPoint(node.skillTreeData.Cost);
            node.ResetNode();
        }
        OnReset?.Invoke();

        foreach (var handler in GetComponentsInChildren<UIConnectedHandler>(true))
            handler.RefreshLineColors();
    }
}
