
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
    public float MaxHealth => _entityStat.GetHealthValue();

    [SerializeField] private Slider _slider;

    protected virtual void Awake()
    {
        _entity = GetComponent<Entity>();
        _entityStat = GetComponent<EntityStat>();
        if(_slider == null)
            _slider = GetComponentInChildren<Slider>();
    }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }
    
    protected virtual void Start()
    {
        CurrentHealth = _entityStat.GetHealthValue();
        UpdateHealthBar();
    }
    
  
    public  virtual bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform targetDealDamage)
    {
        if (AttackEnvaded())
        {
            return false ;
        }
        EntityStat entityStatDealDamage = targetDealDamage != null
            ? targetDealDamage.GetComponent<EntityStat>()
            : null;

        float finalDamge;
        float finalElementalDamage;
        // if (entityStatDealDamage != null)
        // {
        //     finalDamge = damage * _entityStat.GetMigiationValue();
        //     Debug.Log(_entityStat.GetMigiationValue());
        //     finalElementalDamage = elementalDamage * _entityStat.GetElementalResitanceValue();
        // }
        // else
        // {
        //     // Attacker has no EntityStat (e.g. a skill object clone) — apply damage as-is.
        //     finalDamge = damage;
        //     finalElementalDamage = elementalDamage;
        // }
        finalDamge = damage * _entityStat.GetMigiationValue();
        DebugCustom.Log(""+_entityStat.GetMigiationValue());
        finalElementalDamage = elementalDamage * _entityStat.GetElementalResitanceValue();
        ReduceHP(finalDamge + finalElementalDamage);

        return true;
    }


    private bool AttackEnvaded() => Random.Range(0f, 100f) < _entityStat.GetEnvasionValue();

    public virtual void ReduceHP(float damage)
    {
        if (CurrentHealth <= 0)
            _entity.Die();
        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        GetComponent<IHitVFX>()?.PlayHitVFX();
        UpdateHealthBar();
        
    }

    /// <summary>Restores <paramref name="amount"/> HP, capped at max health.</summary>
    public void HealHP(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        UpdateHealthBar();
    }

    protected void UpdateHealthBar()
    {
        _slider.value = CurrentHealth / MaxHealth;
    }

    protected void RegenerateHealth()
    {
        if (CurrentHealth >= MaxHealth)
            return;

        float regenAmount = _entityStat.GetHealthRegen();
        if (regenAmount <= 0f)
            return;

        CurrentHealth = Mathf.Min(CurrentHealth + regenAmount, MaxHealth);
        UpdateHealthBar();
    }

    /// <summary>Restores health to its maximum value. Call this when retrieving an entity from a pool.</summary>
    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        UpdateHealthBar();
    }

    /// <summary>Stops the health regeneration loop. Call this on death.</summary>
    public void StopRegen() => CancelInvoke(nameof(RegenerateHealth));

    /// <summary>Directly sets health to <paramref name="amount"/> without triggering damage events. Used by the save system on load.</summary>
    public void RestoreHealth(float amount)
    {
        CurrentHealth = Mathf.Clamp(amount, 0f, MaxHealth);
        UpdateHealthBar();
    }


    

}