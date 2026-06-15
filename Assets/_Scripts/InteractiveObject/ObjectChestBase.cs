using Interfaces;
using UnityEngine;

public class ObjectChestBase : MonoBehaviour, IHit
{
    [SerializeField] protected float currentHealth = 1f;
    [SerializeField] protected Collider2D collider2D;
    [SerializeField] protected Animator _animator;

    protected virtual void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
    }
    public bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform target)
    {
         if(currentHealth <= 0)
        {
            collider2D.enabled = false;
            _animator.SetBool("Open", true);
            DropChestItem();
            return true; 
        }
        return false;
    }

    protected virtual void DropChestItem()
    {
        
    }
}