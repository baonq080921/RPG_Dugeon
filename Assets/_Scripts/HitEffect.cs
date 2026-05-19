using UnityEngine;

/// <summary>
/// Visual effect component for a single hit flash instance.
/// Controlled by <see cref="HitEffectPool"/>.
/// </summary>
public class HitEffect : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public SpriteRenderer sr;
    private Color _originalColor;

    private void Awake()
    {
        _originalColor = sr.color;
    }

    /// <summary>
    /// Triggers the hit animation.
    /// </summary>
    public void EnableHit()
    {
        _animator.SetBool("OnHit", true);
    }

    /// <summary>
    /// Stops the hit animation.
    /// </summary>
    public void DisableHit()
    {
        _animator.SetBool("OnHit", false);
    }

    /// <summary>
    /// Restores the effect's original sprite color.
    /// </summary>
    public void ResetColor()
    {
        sr.color = _originalColor;
    }

    /// <summary>
    /// Applies a random Z rotation to the effect.
    /// </summary>
    public void SetRandomRotation()
    {
        float randomZRotation = Random.Range(0f, 360f);
        transform.localRotation = Quaternion.Euler(0, 0, randomZRotation);
    }

    /// <summary>
    /// Offsets the effect's local position by <paramref name="randomHitOffSet"/>.
    /// </summary>
    public void SetRandomPositionVFX(Vector2 randomHitOffSet, HitEffect hit)
    {
        hit.transform.localPosition = Vector3.zero + (Vector3)randomHitOffSet;
    }
}
