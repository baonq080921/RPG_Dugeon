using UnityEngine;

public enum LineDirection
{
    None, 
    Up,
    UpRight,
    UpLeft,
    Down,
    DownRight,
    DownLeft,
    Right,
    Left
}
public class UIConnectedLine : MonoBehaviour
{
    [SerializeField] private RectTransform _connectedRotationPoint;
    [SerializeField] private RectTransform _line;
    [SerializeField] private RectTransform _childNodeConnectionPoint;


    public void UpdateLineLengthAndRotation(float length, LineDirection lineDirection)
    {
        if (_line == null)
            return;

        bool isConnected = lineDirection != LineDirection.None;
        float lineLength = isConnected ? length : 0f;
        float direction = isConnected ? GetLineDirection(lineDirection) : 0f;
        _line.sizeDelta = new Vector2(lineLength, _line.sizeDelta.y);
        _line.localEulerAngles = new Vector3(0f, 0f, direction);
    }




    // Get the position of the tail connect point to the child node
    public Vector2 GetConnectedPoint(RectTransform rect){
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect.parent as RectTransform,
            _childNodeConnectionPoint.position,
            null,
            out var localPosition
        );
        return localPosition;
    }

    private float GetLineDirection(LineDirection lineDirection)
    {
        switch (lineDirection)
        {
            case LineDirection.Right:
                return 0f;
            case LineDirection.Left:
                return 180f;
            case LineDirection.Up:
                return 90f;
            case LineDirection.UpLeft:
                return 45f;
            case LineDirection.UpRight:
                return 135f;
            case LineDirection.Down:
                return -90f;
            case LineDirection.DownRight:
                return -45f;
            case LineDirection.DownLeft:
                return -135f;
            default:return 0;
        }
    }



}
