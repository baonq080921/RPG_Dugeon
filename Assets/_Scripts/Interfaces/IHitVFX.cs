namespace Interfaces
{
    /// <summary>
    /// Marks an entity that can receive damage.
    /// </summary>
    public interface IHitVFX
    {
        /// <summary>Applies raw damage to this entity.</summary>
        void PlayHitVFX();
    }
}