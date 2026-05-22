namespace Base
{
    /// <summary>
    /// Marker interface for all event structs used with <see cref="EventBus{T}"/>.
    /// Implement this on a struct to define a dispatchable event.
    /// </summary>
    public interface IEvent { }
}
