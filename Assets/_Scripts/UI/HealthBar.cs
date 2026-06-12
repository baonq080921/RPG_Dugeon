using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps the health bar visually upright when the owning entity flips direction.
/// Attach to a child GameObject of any Entity prefab.
/// </summary>
public class HealthBar : MonoBehaviour
{
    private Entity _entity;
    private Slider _slider;

    void Awake()
    {
        _entity = GetComponentInParent<Entity>();
        _slider = GetComponentInChildren<Slider>();
    }

    void OnEnable()
    {
        _entity.OnFlip += HealthBarFlip;
    }

    void OnDisable()
    {
        _entity.OnFlip -= HealthBarFlip;
    }

    // Counter-flip the scale so world scale stays 1 (keeps text/position correct).
    // Also sync Slider.direction because UGUI fill is calculated in canvas-local space:
    // when localScale.x is negative the canvas x-axis is inverted, so the fill extends
    // the wrong way and shows (1 - value). Toggling direction corrects it.
    private void HealthBarFlip()
    {
        var s = transform.localScale;
        s.x = _entity.direction * Mathf.Abs(s.x);
        transform.localScale = s;
        _slider.direction = s.x < 0f
            ? Slider.Direction.RightToLeft
            : Slider.Direction.LeftToRight;
    }
}
