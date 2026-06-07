
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
        _slider = GetComponentInChildren<Slider>();
    }
    
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
        if (entityStatDealDamage != null)
        {
            finalDamge = damage * entityStatDealDamage.GetMigiationValue();
            finalElementalDamage = elementalDamage * entityStatDealDamage.GetElementalResitanceValue();
        }
        else
        {
            // Attacker has no EntityStat (e.g. a skill object clone) — apply damage as-is.
            finalDamge = damage;
            finalElementalDamage = elementalDamage;
        }
        ReduceHP(finalDamge + finalElementalDamage);

        if (CurrentHealth <= 0)
            _entity.Die();

        return true;
    }


    private bool AttackEnvaded() => Random.Range(0f, 100f) < _entityStat.GetEnvasionValue();

    public void ReduceHP(float damage)
    {
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


    

}