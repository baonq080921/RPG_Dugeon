using System.Collections;
using System.Collections.Generic;
using Base;
using Interfaces;
using player;
using UnityEngine;

public class ElectricEffect : MonoBehaviour
{
    
    [SerializeField] private float _damageRadius;
    private Collider2D[] _results ;
    [SerializeField] private bool _isDraw;

    void Awake()
    {
        _results = new Collider2D[20];
    }

    public void DamageTargetInRadius()
    {
        int dectects = Physics2D.OverlapCircleNonAlloc(transform.position,_damageRadius,_results);
        for(int i = 0 ; i < dectects; i++)
        {
            if(_results[i].TryGetComponent<IHit>(out var hit))
            {
                // DebugCustom.Log("Electric passive hit enemy "+ ServiceLocator.Get<Player>().entityStat.GetElementalDamageDisplay()*1.1f);
                hit?.TakeDamage(0,ServiceLocator.Get<Player>().entityStat.GetElementalDamageValue(out _)*1.1f,ElementType.Electric,transform);

            }
        }
    }

    void OnDrawGizmos()
    {
        if(!_isDraw) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }
}
