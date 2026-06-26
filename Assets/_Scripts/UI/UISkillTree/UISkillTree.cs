using System.Collections.Generic;
using Base;
using Interfaces;
using Inventory;
using player;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UISkillTree : MonoBehaviour, ISkillTreeState
    {
        private PlayerInventory _playerInventory;
        [SerializeField] private TextMeshProUGUI _skillPointTmp;

        public float SkillPoints => _playerInventory?.SkillPoints ?? 0f;

        private void OnEnable()
        {
            Player.ActivePlayerChanged += OnPlayerChanged;
            if (Player.ActivePlayer != null)
                OnPlayerChanged(Player.ActivePlayer);
        }

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
            EventBus<SkillTreeResetEvent>.Raise(new SkillTreeResetEvent());

            foreach (var handler in GetComponentsInChildren<UIConnectedHandler>(true))
                handler.RefreshLineColors();
        }

        /// <inheritdoc/>
        public List<string> GetUnlockedNodeKeys()
        {
            var keys = new List<string>();
            foreach (var node in GetComponentsInChildren<UITreeNode>(true))
                if (node.skillTreeData != null && node.isUnlocked)
                    keys.Add(node.skillTreeData.name);
            return keys;
        }

        /// <inheritdoc/>
        public void RestoreUnlockedNodes(IEnumerable<string> nodeKeys)
        {
            var allNodes = GetComponentsInChildren<UITreeNode>(true);
            foreach (var node in allNodes)
                node.ResetNode();

            var nodeMap = new Dictionary<string, UITreeNode>();
            foreach (var node in allNodes)
                if (node.skillTreeData != null)
                    nodeMap[node.skillTreeData.name] = node;

            foreach (var key in nodeKeys)
                if (nodeMap.TryGetValue(key, out var node))
                    node.RestoreUnlocked();

            foreach (var handler in GetComponentsInChildren<UIConnectedHandler>(true))
                handler.RefreshLineColors();
        }
    }
}
