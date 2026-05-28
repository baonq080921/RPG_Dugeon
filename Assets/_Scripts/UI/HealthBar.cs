using UnityEngine;

/// <summary>
/// Keeps the health bar visually upright when the owning entity flips direction.
/// Attach to a child GameObject of any Entity prefab.
/// </summary>
public class HealthBar : MonoBehaviour
{
    private Entity _entity;

    void Awake()
    {
        _entity = GetComponentInParent<Entity>();
    }

    void OnEnable()
    {
        _entity.OnFlip += HealthBarFlip;
    }

    void OnDisable()
    {
        _entity.OnFlip -= HealthBarFlip;
    }

    // Negate localScale.x to cancel the parent's flip.
    // Parent x=-1, child x=-1 → world x = (-1)*(-1) = 1 (unflipped).
    private void HealthBarFlip()
    {
        var s = transform.localScale;
        s.x = _entity.direction * Mathf.Abs(s.x);
        transform.localScale = s;
    }
}
