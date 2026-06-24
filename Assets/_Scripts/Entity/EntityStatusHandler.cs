using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatusHandler : MonoBehaviour
{
    private ElementType currentElementalStatus = ElementType.None;

    [SerializeField] private GameObject _electricEffectPrefab;
    [SerializeField] private float _currentCharge;
    [SerializeField] private float _maxiumCharge =1f;
    [Header("Reference")]
    private EntityHealth _entityHealth;
    private EntityVfx _entityVfx;
    private Entity _entity;


    [Header("Electric Strike")]
    [SerializeField] private float _shockDuration = 2f;

    [Header("Ice Effect")]
    [SerializeField] private float _iceDuration = 3f;

    [Header("Coroutine")]
    private Coroutine _electricEffectCoroutine;
    private Coroutine _iceEffectCoroutine;
    private Coroutine _draculaDoTCoroutine;

    void Awake()
    {
        _entityHealth = GetComponent<EntityHealth>();
        _entityVfx = GetComponent<EntityVfx>();
        _entity = GetComponent<Entity>();
    }

    #region  Electric Effect
    public void ApplyElectricEffect(float duration,float damage, float charge)
    {
        _currentCharge =_currentCharge + charge;
        Debug.Log($"Current Charge: {_currentCharge}/{_maxiumCharge}");
        if(_currentCharge >=_maxiumCharge)
        {
            DoElectricStrike(damage);
            StopElectricStatusEffects();
            return;
        }

        //if player dont reach max charge, we can apply some visual effect to indicate the current charge level, for example changing the color of the entity or showing a particle effect.
        // StartCoroutine(ElectricEffectDuration(duration));
        if(_electricEffectCoroutine != null)
            StopCoroutine(_electricEffectCoroutine);
        _electricEffectCoroutine = StartCoroutine(ElectricEffectDuration(duration));
    }


    private IEnumerator ElectricEffectDuration(float duration)
    {
        // Apply visual effect for electric status here (e.g., change color, play particle effect)
        currentElementalStatus = ElementType.Electric;
        _entityVfx.UpdateStatusEffectVFX(ElementType.Electric, duration);
        yield return new WaitForSeconds(duration);
        StopElectricStatusEffects();
    }


    private void StopElectricStatusEffects()
    {
        currentElementalStatus = ElementType.None;
        _currentCharge = 0f;

    }

    private void DoElectricStrike(float damage)
    {
        Instantiate(_electricEffectPrefab, transform.position, Quaternion.identity);
        _entityHealth.ReduceHP(damage);
        _entity.Shock(_shockDuration);
        _entityVfx.UpdateStatusEffectVFX(ElementType.Electric, _shockDuration);
    }
    #endregion


    public void ApplyIceEffect(ElementType elementType, float slowPercent)
    {
        if (_iceEffectCoroutine != null)
        {
            StopCoroutine(_iceEffectCoroutine);
            _entity.ResetEffect();
        }
        _entityVfx.UpdateStatusEffectVFX(elementType, _iceDuration);
        _entity.ApplyEffect(slowPercent, elementType);
        _iceEffectCoroutine = StartCoroutine(IceEffectCoroutine());
    }

    private IEnumerator IceEffectCoroutine()
    {
        yield return new WaitForSeconds(_iceDuration);
        _entity.ResetEffect();
        _iceEffectCoroutine = null;
    }

    #region Dracula DoT
    /// <summary>Starts a damage-over-time bleed effect. Re-applying resets the duration.</summary>
    public void ApplyDraculaDoT(float damagePerTick, float duration, float tickInterval,ElementType elementType)
    {
        if (_draculaDoTCoroutine != null)
            StopCoroutine(_draculaDoTCoroutine);
        _entityVfx.UpdateStatusEffectVFX(elementType,duration);
        _draculaDoTCoroutine = StartCoroutine(DraculaDoTCoroutine(damagePerTick, duration, tickInterval));
    }

    private IEnumerator DraculaDoTCoroutine(float damagePerTick, float duration, float tickInterval)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
            _entityHealth.ReduceHP(damagePerTick);
        }
        _draculaDoTCoroutine = null;
    }
    #endregion

    public bool CanElementalStatusApply(ElementType newStatus)
    {
        return currentElementalStatus == ElementType.None || currentElementalStatus == newStatus;
    }
}
