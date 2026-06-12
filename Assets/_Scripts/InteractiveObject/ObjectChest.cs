using Interfaces;
using UnityEngine;

public class ObjectChest : MonoBehaviour, IHit
{
    [SerializeField]private EntityDrop _entityDrop;
    [SerializeField] private float _currentHealth = 30f;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private Animator _animator;

    void Awake()
    {
        _entityDrop = GetComponent<EntityDrop>();
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
    }
    public bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform target)
    {
        _currentHealth -= damage;
        if(_currentHealth <= 0)
        {
            _collider2D.enabled = false;
            _animator.SetBool("Open", true);
            _entityDrop.DropItems();
            return true; 
        }
        return false;
    }
}