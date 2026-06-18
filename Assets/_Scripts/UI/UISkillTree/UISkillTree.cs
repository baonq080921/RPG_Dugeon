using System;
using player;
using TMPro;
using UnityEngine;

public class UISkillTree : MonoBehaviour
{
    public static event Action OnReset;

    private PlayerInventory _playerInventory;
    [SerializeField] private TextMeshProUGUI _skillPointTmp;

    public float SkillPoints => _playerInventory?.SkillPoints ?? 0f;

    private void OnEnable() => Player.ActivePlayerChanged += OnPlayerChanged;

    private void OnDisable()
    {
        Player.ActivePlayerChanged -= OnPlayerChanged;
        if (_playerInventory != null)
            _playerInventory.OnSkillPointsChanged -= UpdateSkillPointText;
    }

    private void Start() => UpdateSkillPointText(SkillPoints);

    private void OnPlayerChanged(Player player)
    {
        if (_playerInventory != null)
            _playerInventory.OnSkillPointsChanged -= UpdateSkillPointText;

        _playerInventory = player.playerInventory;

        if (_playerInventory != null)
            _playerInventory.OnSkillPointsChanged += UpdateSkillPointText;

        UpdateSkillPointText(SkillPoints);
    }

    private void UpdateSkillPointText(float points) => _skillPointTmp.text = $"Skill Point: {points}";

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
