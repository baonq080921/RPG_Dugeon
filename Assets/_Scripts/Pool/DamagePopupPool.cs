using Base;
using UnityEngine;

/// <summary>
/// Pool for <see cref="DamagePopup"/> instances. Listens for <see cref="DamagePopupEvent"/>
/// and spawns a floating damage number at the requested world position.
/// Assigned to <see cref="PoolManager"/> in the Inspector — do not register with ServiceLocator directly.
/// </summary>
public class DamagePopupPool : MonoBehaviourPool<DamagePopup>
{
    [SerializeField] private DamagePopup _prefab;

    private EventBinding<DamagePopupEvent> _eventBinding;

    protected override void Awake()
    {
        base.Awake();
        var manager = ServiceLocator.Get<PoolManager>();
        if (manager != null) manager.damagePopupPool = this;
    }

    private void OnEnable()
    {
        _eventBinding = new EventBinding<DamagePopupEvent>(OnDamagePopup);
        EventBus<DamagePopupEvent>.Register(_eventBinding);
    }

    private void OnDisable()
    {
        EventBus<DamagePopupEvent>.Deregister(_eventBinding);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        var manager = ServiceLocator.Get<PoolManager>();
        if (manager != null && manager.damagePopupPool == this)
            manager.damagePopupPool = null;
    }

    /// <inheritdoc/>
    protected override DamagePopup CreateInstance()
    {
        var item = Instantiate(_prefab, transform);
        item.gameObject.SetActive(false);
        return item;
    }

    /// <inheritdoc/>
    protected override void OnGet(DamagePopup item)
    {
        item.OnComplete = () => Release(item);
    }

    /// <inheritdoc/>
    protected override void OnRelease(DamagePopup item)
    {
        item.OnComplete = null;
        item.transform.SetParent(transform);
    }

    /// <summary>Spawns a floating damage number at <paramref name="worldPosition"/>.</summary>
    public void Spawn(Vector3 worldPosition, float damage, bool isCrit)
    {
        var item = Get();
        item.Play(worldPosition, damage, isCrit);
    }

    private void OnDamagePopup(DamagePopupEvent evt)
    {
        Spawn(evt.WorldPosition, evt.Damage, evt.IsCrit);
    }
}
