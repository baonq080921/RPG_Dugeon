using System.Collections.Generic;
using UnityEngine;
public class EntityCombat : MonoBehaviour
{
    
    [SerializeField] protected List<Collider2D> targetCollider;

    public virtual void AttackTargetDectected()
    {
        
    }
}