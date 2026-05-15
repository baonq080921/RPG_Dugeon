using Base;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public abstract class EntityHealth : MonoBehaviour, IHit
{     
    public float CurrentHealth { get; private set; }
    private Entity _entity;
    /// <summary>Maximum health sourced from the entity's ScriptableObject data.</summary>
    protected abstract float MaxHealth { get; }

    [SerializeField] private Slider _slider;


    protected virtual void Awake()
    {
        _entity = GetComponent<Entity>();
    }
    
    void Start()
    {
        CurrentHealth = MaxHealth;
        _slider.value = CurrentHealth/MaxHealth;
    }
    
  
    public virtual void TakeDamage(float damage, bool applyKnockBack)
    {
        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        GetComponent<IHitVFX>()?.PlayHitVFX();
        _slider.value = CurrentHealth/MaxHealth;
        // DebugCustom.Log($"{name} took {damage} damage. HP: {CurrentHealth}/{MaxHealth}");
        if (CurrentHealth <= 0)
            _entity.Die();

    }

    /// <summary>Restores health to its maximum value. Call this when retrieving an entity from a pool.</summary>
    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }

}