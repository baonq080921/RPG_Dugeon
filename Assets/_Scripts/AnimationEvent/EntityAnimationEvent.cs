using Base;
using UnityEngine;

namespace player
{
    public  class EntityAnimationEvent : MonoBehaviour
    {
        private Entity _enity;
        protected virtual void Awake()
        {
            _enity = GetComponentInParent<Entity>();
        }
        public void SetTrigger() => _enity.TriggerAnimationEvent();


        public void AttackTrigger() => DebugCustom.Log("I perform Attack");
    }
}