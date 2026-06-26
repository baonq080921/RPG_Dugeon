using System;
using UnityEngine;

namespace entity
{
    /// <summary>
    /// The contract an item effect needs from the character it is applied to. Implemented by the
    /// player so item-effect data (in the Inventory package) can drive gameplay through this
    /// entity-level abstraction instead of referencing the concrete <c>player.Player</c> type.
    /// Entity-level components (stat, health, vfx, combat) are reached via <see cref="Entity"/>.
    /// </summary>
    public interface IItemEffectTarget
    {
        /// <summary>The underlying entity, exposing its stat/health/vfx/combat components and transform.</summary>
        public Entity Entity { get; }

        /// <summary>Raised when the target performs an attack that lands.</summary>
        event Action OnAttacking;

        /// <summary>Raised for each enemy transform the target's attack hits.</summary>
        event Action<Transform> OnHitEnemy;

        /// <summary>Raised when the target takes damage.</summary>
        event Action OnTookDamage;
    }
}
