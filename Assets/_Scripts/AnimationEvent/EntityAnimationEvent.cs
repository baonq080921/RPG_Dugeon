using entity;
using UnityEngine;

// Shared base for entity animation events. Kept in the global namespace so both player and enemy
// animation events can derive from it without the enemy namespace depending on the player namespace.
public class EntityAnimationEvent : MonoBehaviour
{
    private Entity _enity;
    private EntityCombat _entityCombat;
    protected virtual void Awake()
    {
        _enity = GetComponentInParent<Entity>();
        _entityCombat = GetComponentInParent<EntityCombat>();
    }
    public void SetTrigger() => _enity.TriggerAnimationEvent();


    public void AttackTrigger() => _entityCombat.PerformedAttack();
}
