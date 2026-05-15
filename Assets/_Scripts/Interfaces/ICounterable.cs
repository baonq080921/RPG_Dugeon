namespace Interfaces
{
    /// <summary>
    /// Marks an entity that can receive damage.
    /// </summary>
    public interface ICounterable
    {
        public bool CanCounter{get; set;}
         void EnableCounter();
         void DisableCounter();

         public void HandleCounter();       
    }
}