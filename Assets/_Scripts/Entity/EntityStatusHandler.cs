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


    [Header("Coroutine")]
    private Coroutine _electricEffectCoroutine;

    void Awake()
    {
        _entityHealth = GetComponent<EntityHealth>();
        _entityVfx = GetComponent<EntityVfx>();
    }


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
        Debug.Log(transform.name + " is struck by electric shock!");
        _entityHealth.ReduceHP(damage);
    }

    public bool CanElementalStatusApply(ElementType newStatus)
    {
        return currentElementalStatus == ElementType.None || currentElementalStatus == newStatus;
    }
}
