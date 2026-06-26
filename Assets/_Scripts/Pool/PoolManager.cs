using Base;
using Effect;
using Inventory;
using UnityEngine;

namespace Pool
{
    public class PoolManager : MonoBehaviour
    {

        public AfterImageEffect afterEffectPool;

        public ItemPickablePool itemObjectPool;

        public LevelUpEffectPool levelupPool;
        public HitEffectPool hitEffectPool;
        public SliceEffectPool sliceEffectPool;
        public DamagePopupPool damagePopupPool;
        public ElectricEffectPool electricEffectPool;

        private EventBinding<DropItemRequestedEvent> _dropItemBinding;

        private void Awake()
        {
            ServiceLocator.Register<PoolManager>(this);
        }

        private void OnEnable()
        {
            _dropItemBinding = new EventBinding<DropItemRequestedEvent>(HandleDropItemRequested);
            EventBus<DropItemRequestedEvent>.Register(_dropItemBinding);
        }

        private void OnDisable()
        {
            EventBus<DropItemRequestedEvent>.Deregister(_dropItemBinding);
        }

        // The Inventory/entity layers raise DropItemRequestedEvent instead of referencing this pool directly,
        // which keeps the Pool dependency out of those lower layers (inverts the Inventory→Pool / entity→Pool edges).
        private void HandleDropItemRequested(DropItemRequestedEvent e)
        {
            itemObjectPool?.Spawn(e.itemData, e.position);
        }
    }
}
