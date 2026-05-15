using Base;
using UnityEngine;

namespace player
{
    public  class EntityAnimationEvent : MonoBehaviour
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
}