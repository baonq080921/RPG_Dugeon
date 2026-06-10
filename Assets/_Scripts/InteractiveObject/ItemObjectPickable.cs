
using Interfaces;
using UnityEngine;

public class ItemObjectPickable : MonoBehaviour, ICollectable
{
    [field:SerializeField] public ItemData itemData{get; private set;}
    [SerializeField] private LayerMask _hitLayer;
    [SerializeField] private SpriteRenderer _sr;
    private InventoryBase _playerInventory;
    private ItemInventory _itemInventory;


    void Awake()
    {
        _itemInventory = new ItemInventory(itemData);
    }
    protected virtual void OnValidate()
    {
        if(itemData == null) return;
        _sr.sprite = itemData.Sprite;
        gameObject.name = $"Item -{itemData.ItemName}";
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if((_hitLayer.value &(1 << other.gameObject.layer)) == 0) return;
        _playerInventory = other.GetComponent<InventoryBase>();
        CollectableObject();
    }

    public void CollectableObject()
    {
        if(!_playerInventory.CanAddToIventory()) return;
        Debug.Log("Collect this item");
        _playerInventory.AddToInventory(_itemInventory);
        gameObject.SetActive(false);
    }
}
