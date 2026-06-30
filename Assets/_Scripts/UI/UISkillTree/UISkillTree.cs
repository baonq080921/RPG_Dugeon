using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using player;
using SaveData;
using TMPro;
using UnityEngine;

public class UISkillTree : MonoBehaviour, ITreeNodePersistent
{
    public static event Action OnReset;

    private PlayerInventory _playerInventory;
    private EventBinding<SkillTreeRestoreEvent> _restoreBinding;
    [SerializeField] private TextMeshProUGUI _skillPointTmp;

    public float SkillPoints => _playerInventory?.SkillPoints ?? 0f;

    void Awake()
    {
        ServiceLocator.Register<ITreeNodePersistent>(this);
    }

    private void OnEnable()
    {
        Player.ActivePlayerChanged += OnPlayerChanged;
        if (Player.ActivePlayer != null)
            OnPlayerChanged(Player.ActivePlayer);

        _restoreBinding = new EventBinding<SkillTreeRestoreEvent>(OnRestoreSkillTree);
        EventBus<SkillTreeRestoreEvent>.Register(_restoreBinding);
    }

    private void OnDisable()
    {
        Player.ActivePlayerChanged -= OnPlayerChanged;
        if (_playerInventory != null)
            _playerInventory.OnSkillPointsChanged -= UpdateSkillPointText;

        EventBus<SkillTreeRestoreEvent>.Deregister(_restoreBinding);
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

    private void OnRestoreSkillTree(SkillTreeRestoreEvent e)
    {
        var allNodes = GetComponentsInChildren<UITreeNode>(true);
        foreach (var node in allNodes)
            node.ResetNode();

        RestoreSkillPoints(e.SkillPoints);

        var nodeMap = new Dictionary<string, UITreeNode>();
        foreach (var node in allNodes)
            if (node.skillTreeData != null)
                nodeMap[node.skillTreeData.name] = node;

        foreach (var entry in e.SkillTree.nodes)
        {
            if (nodeMap.TryGetValue(entry.nodeKey, out var node))
                node.RestoreUnlocked();
            else
                Debug.LogWarning($"[UISkillTree] Unknown skill tree node '{entry.nodeKey}' — skipped.");
        }

        foreach (var handler in GetComponentsInChildren<UIConnectedHandler>(true))
            handler.RefreshLineColors();
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

    public List<UITreeNode> GetUINodesInSkillTree()
    {
        return transform.GetComponentsInChildren<UITreeNode>().ToList();
    }
}

