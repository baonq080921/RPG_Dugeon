using UnityEngine;

public class UISkillTree : MonoBehaviour
{
    [SerializeField] private float _skillPoint;

    public bool EnoughSkillPoint(float cost) => _skillPoint >= cost;

    public void ReduceSkillPoint(float cost)
    {
        if (!EnoughSkillPoint(cost)) return;
        _skillPoint -= cost;
    }

    public void RefundSkillPoint(float cost) => _skillPoint += cost;




    /// <summary>
    /// Arranges all child nodes by applying line geometry then repositioning each node
    /// to its connection point. Run this after assigning line lengths and offsets.
    /// </summary>
    [ContextMenu("Arrange Skill Tree Nodes")]
    public void ArrangeSkillTreeNodes()
    {
        foreach (var handler in GetComponentsInChildren<UIConnectedHandler>())
            handler.ArrangeChildNodes();
    }

    [ContextMenu("Reset Skill Tree")]
    /// <summary>
    /// Refunds all spent skill points and resets every node in the tree back to its initial locked state.
    /// </summary>
    public void ResetSkillTree()
    {
        UITreeNode[] allNodes = GetComponentsInChildren<UITreeNode>();

        foreach (var node in allNodes)
        {
            if (node.isUnlocked && node.skillTreeData != null)
                RefundSkillPoint(node.skillTreeData.Cost);
            node.ResetNode();
        }

        foreach (var handler in GetComponentsInChildren<UIConnectedHandler>())
            handler.RefreshLineColors();
    }
}
