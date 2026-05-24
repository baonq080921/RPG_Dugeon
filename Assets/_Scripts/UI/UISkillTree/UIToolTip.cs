using UnityEngine;

public class UIToolTip : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float _edgeGap = 8f;

    private static readonly Vector3 HiddenPos = new Vector3(9999f, 9999f, 0f);

    void Awake()
    {
        rectTransform.position = HiddenPos;
    }

    /// <summary>
    /// Shows or hides the tooltip. When showing, animates to an edge-aware position near <paramref name="rect"/>.
    /// </summary>
    public virtual void ShowToolTip(bool isShow, RectTransform rect)
    {
        if (!isShow)
        {
            HideToolTip();
            return;
        }

        MoveToTarget(rect);
    }

    private void MoveToTarget(RectTransform rect)
    {
        rectTransform.position = GetAdjustedPosition(rect);
    }

    /// <summary>
    /// Returns the tooltip world position adjusted so it stays fully on screen,
    /// flipping horizontally based on screen half and clamping vertically so it
    /// never overflows the top or bottom edge.
    /// </summary>
    private Vector3 GetAdjustedPosition(RectTransform rect)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, rect.position);
        Vector3 targetPos = rect.position;

        // Horizontal: flip to opposite side based on screen half
        float horizontalOffset = (rectTransform.rect.width + rect.rect.width) * 0.5f + _edgeGap;
        if (screenPos.x > Screen.width * 0.5f)
            targetPos.x -= horizontalOffset;
        else
            targetPos.x += horizontalOffset;

        // Vertical: clamp so tooltip doesn't overflow top or bottom
        float tooltipHalfHeight = rectTransform.rect.height * 0.5f;
        float minScreenY = tooltipHalfHeight + _edgeGap;
        float maxScreenY = Screen.height - tooltipHalfHeight - _edgeGap;
        float clampedScreenY = Mathf.Clamp(screenPos.y, minScreenY, maxScreenY);

        // Convert the clamped screen Y back to world space
        Vector2 clampedScreenPos = new Vector2(screenPos.x, clampedScreenY);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, clampedScreenPos, null, out Vector3 clampedWorldPos);
        targetPos.y = clampedWorldPos.y;

        return targetPos;
    }

    private void HideToolTip()
    {
        rectTransform.position = HiddenPos;
    }
}
