using System;
using Base;
using UnityEngine;

/// <summary>
/// Place this component on the <see cref="EnemyArcher"/> prefab.
/// Each archer owns its own pool — no global dependency on <see cref="PoolManager"/>.
/// </summary>
public class ArcherArrowPool : MonoBehaviour
{
    [SerializeField] private ArcherArrowObject _arrowPrefab;
    [SerializeField] private ArcherArrowIceObject _arrowIcePrefab;
    [SerializeField] private int _defaultCapacity = 5;
    [SerializeField] private int _maxSize = 20;

    private ObjectPool<ArcherArrowObject> _arrowPool;
    private ObjectPool<ArcherArrowIceObject> _iceArrowPool;

    private void Awake()
    {
        _arrowPool = new ObjectPool<ArcherArrowObject>(
            createFunc: () => { var a = Instantiate(_arrowPrefab); a.gameObject.SetActive(false); return a; },
            actionOnGet: a => a.gameObject.SetActive(true),
            actionOnRelease: a => { a.ResetArrow(); a.gameObject.SetActive(false); },
            actionOnDestroy: a => { if (a != null) Destroy(a.gameObject); },
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize);

        _iceArrowPool = new ObjectPool<ArcherArrowIceObject>(
            createFunc: () => { var a = Instantiate(_arrowIcePrefab); a.gameObject.SetActive(false); return a; },
            actionOnGet: a => a.gameObject.SetActive(true),
            actionOnRelease: a => { a.ResetArrow(); a.gameObject.SetActive(false); },
            actionOnDestroy: a => { if (a != null) Destroy(a.gameObject); },
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize);
    }

    private void OnDestroy()
    {
        _arrowPool?.Dispose();
        _iceArrowPool?.Dispose();
    }

    /// <summary>Gets a pooled normal arrow and binds its release callback.</summary>
    public ArcherArrowObject GetArrow()
    {
        var arrow = _arrowPool.Get();
        arrow.SetPool(() => _arrowPool.Release(arrow));
        return arrow;
    }

    /// <summary>Gets a pooled ice arrow and binds its release callback.</summary>
    public ArcherArrowIceObject GetIceArrow()
    {
        var arrow = _iceArrowPool.Get();
        arrow.SetPool(() => _iceArrowPool.Release(arrow));
        return arrow;
    }
}
