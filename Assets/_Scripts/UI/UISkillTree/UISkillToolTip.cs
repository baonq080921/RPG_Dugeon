using System.Text;
using Base;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UISkillToolTip : UIToolTip 
{
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillDescriptionText;
    [SerializeField] private TextMeshProUGUI _skillRequiredLevelText;

    [SerializeField] private string _metConditionColorHex = "#3EF312";
    [SerializeField] private string _unMetConditionColorHex = "#F30909";
    private string _lockText = "You choose the other path skill";
    public override void ShowToolTip(bool isShow, RectTransform rect)
    {
        base.ShowToolTip(isShow, rect);
    }



    public void ShowToolTip(bool isShow, RectTransform rect,UITreeNode node)
    {
        base.ShowToolTip(isShow, rect);
        float cost = node.skillTreeData.Cost;
        _skillNameText.text = node.skillTreeData.skillTreeDataName;
        _skillDescriptionText.text = node.skillTreeData.Description;
        string skillLockText = node.isLocked ? _lockText : GetRequirement(cost,node.requireNode, node.confilctNode);
        _skillRequiredLevelText.text = skillLockText;
    }


    private string GetRequirement(float cost, UITreeNode[] requireNode, UITreeNode[] conflictNode)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Requirements: ");
        string costStringColorHex = ServiceLocator.Get<UIManager>().uISkillTree.EnoughSkillPoint(cost)? _metConditionColorHex : _unMetConditionColorHex;
        sb.AppendLine($"<color={costStringColorHex}>- {cost} skillPoint(s)</color>");
        foreach(var node in requireNode)
        {
            if(node == null) continue;
            string requireNodeUnlockColorHex = node.isUnlocked ? _metConditionColorHex : _unMetConditionColorHex;
            sb.AppendLine($"<color={requireNodeUnlockColorHex}>{node.skillTreeData.skillTreeDataName}</color>");
        }
        sb.AppendLine("");
        sb.AppendLine("-Locked out: ");
        foreach(var node in conflictNode)
        {
            if(node == null) continue;
            sb.AppendLine($"-{node.skillTreeData.skillTreeDataName}");
        }
        return sb.ToString();
    }
}