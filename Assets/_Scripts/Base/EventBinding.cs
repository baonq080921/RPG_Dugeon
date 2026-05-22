using System;

namespace Base
{
    /// <summary>
    /// Internal contract used by <see cref="EventBus{T}"/> to invoke listeners
    /// without exposing the setters publicly.
    /// </summary>
    internal interface IEventBinding<T>
    {
        Action<T> OnEvent { get; set; }
        Action OnEventNoArgs { get; set; }
    }

    /// <summary>
    /// A handle that binds one or two listeners to an event type.
    /// Pass an <c>Action&lt;T&gt;</c> to receive event data, or a plain <c>Action</c>
    /// if you only care that the event fired.
    /// Register and deregister with <see cref="EventBus{T}"/>.
    /// </summary>
    /// <typeparam name="T">An <see cref="IEvent"/> struct that carries the event payload.</typeparam>
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> _onEvent = _ => { };
        private Action _onEventNoArgs = () => { };

        Action<T> IEventBinding<T>.OnEvent
        {
            get => _onEvent;
            set => _onEvent = value;
        }

        Action IEventBinding<T>.OnEventNoArgs
        {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        /// <summary>Creates a binding that receives the event payload.</summary>
        public EventBinding(Action<T> onEvent) => _onEvent = onEvent;

        /// <summary>Creates a binding that is notified without receiving payload data.</summary>
        public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs;

        /// <summary>Adds an additional payload listener to this binding.</summary>
        public void Add(Action<T> onEvent) => _onEvent += onEvent;

        /// <summary>Removes a payload listener from this binding.</summary>
        public void Remove(Action<T> onEvent) => _onEvent -= onEvent;

        /// <summary>Adds an additional no-arg listener to this binding.</summary>
        public void Add(Action onEventNoArgs) => _onEventNoArgs += onEventNoArgs;

        /// <summary>Removes a no-arg listener from this binding.</summary>
        public void Remove(Action onEventNoArgs) => _onEventNoArgs -= onEventNoArgs;
    }
}
