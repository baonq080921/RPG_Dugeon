using Base;
using Interfaces;
using Inventory;
using UnityEngine;
namespace InteractiveObject
{
    public class ItemObjectPickable : MonoBehaviour, ICollectable
    {
        [field: SerializeField] public ItemData itemData { get; private set; }
        [SerializeField] private LayerMask _hitLayer;
        [SerializeField] private SpriteRenderer _sr;
        private InventoryBase _playerInventory;
        private ItemInventory _itemInventory;
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        private float _shootPower = 4f;
        [SerializeField] private LayerMask _groundLayer;
        public System.Action OnComplete;


        void Awake()
        {
            _itemInventory = new ItemInventory(itemData);
        }

        /// <summary>
        /// Sets item data at runtime for dynamically spawned pickables (e.g. crafted items).
        /// </summary>
        public void Initialize(ItemData data)
        {
            itemData = data;
            _sr.sprite = data.Sprite;
            gameObject.name = $"Item -{data.ItemName}";
            _itemInventory = new ItemInventory(data);
        }

        protected virtual void OnValidate()
        {
            if (itemData == null) return;
            _sr.sprite = itemData.Sprite;
            gameObject.name = $"Item -{itemData.ItemName}";
        }

        public void ShootItem()
        {
            _collider2D.isTrigger = false;
            _collider2D.excludeLayers = _hitLayer; // ignore player while airborne
            float randomX = Random.Range(-_shootPower, _shootPower);
            _rigidbody2D.velocity = new Vector2(randomX, _shootPower);
        }


        void OnCollisionEnter2D(Collision2D collision)
        {
            if (((1 << collision.gameObject.layer) & _groundLayer) != 0)
            {
                _collider2D.excludeLayers = 0;
                _collider2D.isTrigger = true;
                _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if ((_hitLayer.value & (1 << other.gameObject.layer)) == 0) return;
            _playerInventory = other.GetComponent<InventoryBase>();
            CollectableObject();
        }

        public void CollectableObject()
        {
            if (!_playerInventory.CanAddToIventory()) return;
            _playerInventory.AddToInventory(_itemInventory);
            OnComplete?.Invoke();

        }

        public void ResetState()
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.constraints = RigidbodyConstraints2D.None;
            _collider2D.isTrigger = false;
            _collider2D.excludeLayers = 0;
            _playerInventory = null;
        }
    }
}
