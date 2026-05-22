using System.Collections.Generic;

namespace Base
{
    /// <summary>
    /// Global type-safe event bus. Each event type <typeparamref name="T"/> has its own
    /// isolated channel — raising <c>PlayerDiedEvent</c> never touches <c>EnemyDiedEvent</c> listeners.
    /// </summary>
    /// <typeparam name="T">An <see cref="IEvent"/> struct that carries the event payload.</typeparam>
    /// <example>
    /// Define an event:
    /// <code>
    /// public struct PlayerDiedEvent : IEvent { }
    /// </code>
    /// Subscribe (e.g. in OnEnable):
    /// <code>
    /// _binding = new EventBinding&lt;PlayerDiedEvent&gt;(OnPlayerDied);
    /// EventBus&lt;PlayerDiedEvent&gt;.Register(_binding);
    /// </code>
    /// Unsubscribe (e.g. in OnDisable):
    /// <code>
    /// EventBus&lt;PlayerDiedEvent&gt;.Deregister(_binding);
    /// </code>
    /// Raise:
    /// <code>
    /// EventBus&lt;PlayerDiedEvent&gt;.Raise(new PlayerDiedEvent());
    /// </code>
    /// </example>
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> _bindings = new HashSet<IEventBinding<T>>();

        /// <summary>Registers a binding to receive future events of type <typeparamref name="T"/>.</summary>
        public static void Register(EventBinding<T> binding) => _bindings.Add(binding);

        /// <summary>Removes a previously registered binding.</summary>
        public static void Deregister(EventBinding<T> binding) => _bindings.Remove(binding);

        /// <summary>Raises the event, invoking all registered bindings.</summary>
        public static void Raise(T @event)
        {
            foreach (var binding in _bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }
    }
}
