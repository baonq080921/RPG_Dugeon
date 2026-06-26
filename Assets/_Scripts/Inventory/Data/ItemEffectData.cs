using entity;
using UnityEngine;

namespace Inventory
{
    /// <summary>Base ScriptableObject for item effects that trigger on game events.</summary>
    public class ItemEffectData : ScriptableObject
    {
        protected IItemEffectTarget target;
        [field: TextArea]
        [field: SerializeField] public string effectInfo { get; private set; }
        protected virtual void ExecuteEffect() { }

        /// <summary>Subscribes this effect to the relevant events for <paramref name="target"/>.</summary>
        public virtual void Subscribe(IItemEffectTarget target) { }

        /// <summary>Unsubscribes this effect from all events. Call when the item is unequipped.</summary>
        public virtual void Unsubscribe(IItemEffectTarget target) { }
    }
}
