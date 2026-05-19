using UnityEngine;

namespace Interfaces
{
    /// <summary>
    /// Marks an entity that can receive damage.
    /// </summary>
    public interface IHit
    {
        /// <summary>Applies raw damage to this entity.</summary>
        /// <param name="damage">Damage amount</param>
        /// <param name="applyKnockBack">Whether the hit should trigger knockback on the receiver.</param>
        bool TakeDamage(float damage, Transform target);
    }
}