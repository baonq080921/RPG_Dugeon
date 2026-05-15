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
    protected float MaxHealth { get; }

    [SerializeField] private Slider _slider;

    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private Color _hitColor;


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
    
  
    public  virtual bool TakeDamage(float damage, bool applyKnockBack, Transform target)
    {
        if (AttackEnvaded())
        {
            return false ;
        }
        CaculateDamage(damage);
        CreateHitEffect(target);

        if (CurrentHealth <= 0)
            _entity.Die();

        return true;
    }


    private bool AttackEnvaded() => Random.Range(0f, 100f) < _entityStat.GetEnvasionValue();

    private void CaculateDamage(float damage)
    {
        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        GetComponent<IHitVFX>()?.PlayHitVFX();

        _slider.value = CurrentHealth / MaxHealth;
    }

    /// <summary>Restores health to its maximum value. Call this when retrieving an entity from a pool.</summary>
    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }


    private void CreateHitEffect(Transform target)
    {
        float randomX = Random.Range(-0.3f, 0.3f);
        float randomY = Random.Range(-0.7f, 0.7f);
        Vector2 randomHitOffSet = new Vector2(randomX, randomY);
        ServiceLocator.Get<HitEffectPool>().SpawnHitEffect(target, _hitColor, randomHitOffSet);
    }

}