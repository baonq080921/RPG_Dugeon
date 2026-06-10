
using DG.Tweening;
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
    [SerializeField] private float _healthTweenDuration = 0.3f;
    private Tween _healthTween;




    protected virtual void Awake()
    {
        _entity = GetComponent<Entity>();
        _entityStat = GetComponent<EntityStat>();
        if(_slider == null)
            _slider = GetComponentInChildren<Slider>();
    }
    
    protected virtual void Start()
    {
        CurrentHealth = _entityStat.GetHealthValue();
        SnapHealthBar();
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
        Debug.Log("kadkadjkajdkajd");
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        SnapHealthBar();
    }

    protected void UpdateHealthBar()
    {
        _healthTween?.Kill();
        float target = CurrentHealth / MaxHealth;
        _healthTween = DOTween.To(() => _slider.value, x => _slider.value = x, target, _healthTweenDuration)
            .SetEase(Ease.OutCubic);
    }

    private void SnapHealthBar()
    {
        _healthTween?.Kill();
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
        SnapHealthBar();
    }

    /// <summary>Stops the health regeneration loop. Call this on death.</summary>
    public void StopRegen() => CancelInvoke(nameof(RegenerateHealth));


    

}