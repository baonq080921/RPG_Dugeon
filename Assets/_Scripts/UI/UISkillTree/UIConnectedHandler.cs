using System;
using UnityEngine;
[Serializable]
public struct ConnectedDetails
{
    public UIConnectedHandler childNode;
    [Range(0f, 300f)]public float lineLength;
    public LineDirection lineDirection;
}


public class UIConnectedHandler : MonoBehaviour
{

    private RectTransform _rect;
    public ConnectedDetails[] connectedDetails;
    public UIConnectedLine[] uIConnectedLines;

    void OnValidate()
    {
        if(_rect == null)
            _rect = GetComponent<RectTransform>();
        if(connectedDetails.Length != uIConnectedLines.Length) return;

        UpdateConnectedLines();
    }


    private void UpdateConnectedLines()
    {
        for(int i = 0 ; i < connectedDetails.Length; i++)
        {
            Vector2 targetPosition = uIConnectedLines[i].GetConnectedPoint(_rect);
            uIConnectedLines[i].UpdateLineLengthAndRotation(connectedDetails[i].lineLength, connectedDetails[i].lineDirection);
            if(connectedDetails[i].childNode == null) return; // no child node no need to Set Position
            connectedDetails[i].childNode.SetPosition(targetPosition);
        }
    }


    public void SetPosition(Vector2 pos) => _rect.anchoredPosition = pos;
}

