using Base;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public abstract class EntityHealth : MonoBehaviour, IHit
{     
    [field:SerializeField]public float CurrentHealth { get; private set; }
    private Entity _entity;
    private EntityStat _entityStat;
    /// <summary>Maximum health sourced from the entity's ScriptableObject data.</summary>
    protected float MaxHealth => _entityStat.GetHealthValue();

    [SerializeField] private Slider _slider;

  


    protected virtual void Awake()
    {
        _entity = GetComponent<Entity>();
        _entityStat = GetComponent<EntityStat>();
    }
    
    void Start()
    {
        CurrentHealth = _entityStat.GetHealthValue();
        _slider.value = CurrentHealth/MaxHealth;
    }
    
  
    public  virtual bool TakeDamage(float damage, Transform target)
    {
        if (AttackEnvaded())
        {
            return false ;
        }
        CaculateDamage(damage);
        if (CurrentHealth <= 0)
            _entity.Die();

        return true;
    }


    private bool AttackEnvaded() => Random.Range(0f, 100f) < _entityStat.GetEnvasionValue();

    private void CaculateDamage(float damage)
    {
        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        GetComponent<IHitVFX>()?.PlayHitVFX();
        Debug.Log($"{gameObject.name} took {damage} damage. Current Health: {CurrentHealth}/{MaxHealth}");
        _slider.value = CurrentHealth / MaxHealth;
    }

    /// <summary>Restores health to its maximum value. Call this when retrieving an entity from a pool.</summary>
    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }


    

}