using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// Floating damage number that appears above a target on hit.
/// Animates upward and fades out, then invokes <see cref="OnComplete"/> so its pool can reclaim it.
/// </summary>
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _floatDistance = 1.2f;
    [SerializeField] private float _duration = 0.8f;
    [SerializeField] private float _horizontalJitter = 0.3f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _critColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private float _critScaleMultiplier = 1.4f;

    private Vector3 _baseScale;
    private Sequence _sequence;

    /// <summary>Invoked when the animation finishes so the pool can reclaim this instance.</summary>
    public Action OnComplete;

    private void Awake()
    {
        if (_text == null)
            _text = GetComponentInChildren<TextMeshPro>();
        _baseScale = transform.localScale;
    }

    /// <summary>
    /// Configures this popup at <paramref name="worldPosition"/> with <paramref name="damage"/> and crit styling,
    /// then plays the float-up + fade animation.
    /// </summary>
    public void Play(Vector3 worldPosition, float damage, bool isCrit)
    {
        transform.position = worldPosition;
        _text.text = Mathf.RoundToInt(damage).ToString();
        _text.color = isCrit ? _critColor : _normalColor;
        transform.localScale = _baseScale * (isCrit ? _critScaleMultiplier : 1f);

        _sequence?.Kill();
        _text.alpha = 1f;

        float jitterX = UnityEngine.Random.Range(-_horizontalJitter, _horizontalJitter);
        Vector3 targetPosition = worldPosition + new Vector3(jitterX, _floatDistance, 0f);

        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(targetPosition, _duration).SetEase(Ease.OutCubic));
        _sequence.Join(_text.DOFade(0f, _duration).SetEase(Ease.InCubic));
        _sequence.Join(transform.DOPunchScale(Vector3.one * 0.25f, _duration * 0.4f, 5, 0.5f));
        _sequence.OnComplete(() => OnComplete?.Invoke());
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}
