using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillToolTip : UIToolTip 
{
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillDescriptionText;
    [SerializeField] private TextMeshProUGUI _skillRequiredLevelText;
    public override void ShowToolTip(bool isShow, RectTransform rect)
    {
        base.ShowToolTip(isShow, rect);
    }



    public void ShowToolTip(bool isShow, RectTransform rect, SkillTreeData skillTreeData)
    {
        base.ShowToolTip(isShow, rect);
        _skillNameText.text = skillTreeData.skillTreeDataName;
        _skillDescriptionText.text = skillTreeData.Description;
        _skillRequiredLevelText.text = $"Required to opened: {skillTreeData.Cost} skill points";
    }
}