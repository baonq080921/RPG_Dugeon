using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ConnectedDetails
{
    public UIConnectedHandler childNode;
    [Range(0f, 300f)] public float lineLength;
    [Range(-25f, 100f)] public float offSet;
    public LineDirection lineDirection;
}

public class UIConnectedHandler : MonoBehaviour
{
    private RectTransform _rect;
    public ConnectedDetails[] connectedDetails;
    public UIConnectedLine[] uIConnectedLines;

    public void OnValidate()
    {
        if (_rect == null)
            _rect = GetComponent<RectTransform>();
        if (connectedDetails.Length != uIConnectedLines.Length) return;

        // Only update line geometry in OnValidate; use the context menu to reposition nodes
        UpdateLineGeometry();
    }

    /// <summary>
    /// Resizes and rotates all lines then repositions every child node to the line's
    /// connection point. Call this via the context menu after assigning lengths and offsets.
    /// </summary>
    [ContextMenu("Arrange Child Nodes")]
    public void ArrangeChildNodes()
    {
        if (_rect == null)
            _rect = GetComponent<RectTransform>();
        if (connectedDetails.Length != uIConnectedLines.Length) return;

        // Pass 1: apply line geometry so the canvas has the correct sizes/rotations
        UpdateLineGeometry();

        // Force the canvas layout to recalculate so _childNodeConnectionPoint
        // world positions reflect the new line lengths before we sample them
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

        // Pass 2: sample connection-point positions and move child nodes
        for (int i = 0; i < connectedDetails.Length; i++)
        {
            if (connectedDetails[i].childNode == null || uIConnectedLines[i] == null) continue;
            Vector2 targetPosition = uIConnectedLines[i].GetConnectedPoint(_rect);
            connectedDetails[i].childNode.SetPosition(targetPosition);
        }
    }

    /// <summary>
    /// Updates each line color: white if the child node is unlocked, original color otherwise.
    /// </summary>
    public void RefreshLineColors()
    {
        for (int i = 0; i < connectedDetails.Length; i++)
        {
            if (connectedDetails[i].childNode == null || uIConnectedLines[i] == null) continue;
            UITreeNode childTreeNode = connectedDetails[i].childNode.GetComponent<UITreeNode>();
            bool isUnlocked = childTreeNode != null && childTreeNode.isUnlocked;
            if (isUnlocked)
                uIConnectedLines[i].SetLineColor(Color.white);
            else
                uIConnectedLines[i].ResetLineColor();
        }
    }

    public void SetPosition(Vector2 pos) => _rect.anchoredPosition = pos;

    private void UpdateLineGeometry()
    {
        for (int i = 0; i < connectedDetails.Length; i++)
        {
            if (uIConnectedLines[i] == null) continue;
            uIConnectedLines[i].UpdateLineLengthAndRotation(
                connectedDetails[i].lineLength,
                connectedDetails[i].lineDirection,
                connectedDetails[i].offSet);
        }
    }
}
