using Base;
using UnityEngine;

/// <summary>Pool for <see cref="ItemObjectPickable"/> world drops. Register via ServiceLocator.</summary>
public class ItemPickablePool : MonoBehaviourPool<ItemObjectPickable>
{
    [SerializeField] private ItemObjectPickable _prefab;

    protected override void Awake()
    {
        base.Awake();
        ServiceLocator.Register<ItemPickablePool>(this);
    }

    protected override ItemObjectPickable CreateInstance()
    {
        var item = Instantiate(_prefab);
        item.gameObject.SetActive(false);
        return item;
    }

    protected override void OnGet(ItemObjectPickable item) => item.ResetState();

    /// <summary>Gets a pooled pickup, places it at <paramref name="position"/>, and shoots it.</summary>
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
