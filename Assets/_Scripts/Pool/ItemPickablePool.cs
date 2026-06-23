using Base;
using UnityEngine;

/// <summary>
/// Pool for <see cref="ItemObjectPickable"/> world drops.
/// Per-scene — registers itself with <see cref="PoolManager"/> on Awake so the
/// persistent PoolManager always points to the current scene's pool.
/// </summary>
public class ItemPickablePool : MonoBehaviourPool<ItemObjectPickable>
{
    [SerializeField] private ItemObjectPickable _prefab;

    protected override void Awake()
    {
        base.Awake();
        var manager = ServiceLocator.Get<PoolManager>();
        if (manager != null) manager.itemObjectPool = this;
    }

    protected override void OnDestroy()
    {
        var manager = ServiceLocator.Get<PoolManager>();
        if (manager != null && manager.itemObjectPool == this)
            manager.itemObjectPool = null;
    }

    protected override ItemObjectPickable CreateInstance()
    {
        var item = Instantiate(_prefab);
        item.gameObject.SetActive(false);
        return item;
    }

    protected override void OnGet(ItemObjectPickable item) => item.ResetState();

    /// <summary>Gets a pooled pickup, places it at <paramref name="position"/>, and shoots it out.</summary>
    public void Spawn(ItemData itemData, Vector3 position)
    {
        var item = Get();
        item.transform.position = position;
        item.Initialize(itemData);
        item.ShootItem();
    }

    /// <summary>Returns a pickup to the pool.</summary>
    public void Return(ItemObjectPickable item) => Release(item);
}
