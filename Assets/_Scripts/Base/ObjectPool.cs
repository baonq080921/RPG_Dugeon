using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    /// <summary>
    /// A generic object pool that recycles instances to avoid repeated Instantiate/Destroy GC pressure.
    /// Uses a <see cref="Stack{T}"/> internally for O(1) get and release operations.
    /// </summary>
    /// <typeparam name="T">The type of object to pool. Must be a reference type.</typeparam>
    public class ObjectPool<T> : IDisposable where T : class
    {
        private readonly Stack<T> _stack;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _actionOnGet;
        private readonly Action<T> _actionOnRelease;
        private readonly Action<T> _actionOnDestroy;
        private readonly bool _collectionCheck;
        private readonly int _maxSize;

        private int _countAll;
        private bool _isDisposed;

        /// <summary>Total objects created by this pool (active + inactive).</summary>
        public int CountAll => _countAll;

        /// <summary>Objects currently sitting idle in the pool.</summary>
        public int CountInactive => _stack.Count;

        /// <summary>Objects currently checked out and in use.</summary>
        public int CountActive => _countAll - _stack.Count;

        /// <summary>
        /// Creates a new pool.
        /// </summary>
        /// <param name="createFunc">Called when the pool needs a brand-new instance.</param>
        /// <param name="actionOnGet">Called each time an instance is taken from the pool.</param>
        /// <param name="actionOnRelease">Called each time an instance is returned to the pool.</param>
        /// <param name="actionOnDestroy">Called when an instance is discarded (pool is full or disposed).</param>
        /// <param name="collectionCheck">
        /// When true, logs an error if the same instance is released twice.
        /// Disable in production builds to avoid the O(n) Contains check.
        /// </param>
        /// <param name="defaultCapacity">Initial stack capacity — avoids early resizing, not pre-warming.</param>
        /// <param name="maxSize">Hard cap on inactive instances. Excess objects are destroyed.</param>
        public ObjectPool(
            Func<T> createFunc,
            Action<T> actionOnGet = null,
            Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000)
        {
            if (maxSize <= 0)
                throw new ArgumentException("maxSize must be greater than zero.", nameof(maxSize));

            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _collectionCheck = collectionCheck;
            _maxSize = maxSize;
            _stack = new Stack<T>(defaultCapacity);
        }

        /// <summary>
        /// Returns an instance from the pool, creating a new one if none are available.
        /// </summary>
        public T Get()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            T item;
            if (_stack.Count > 0)
            {
                item = _stack.Pop();
            }
            else
            {
                item = _createFunc();
                _countAll++;
            }

            _actionOnGet?.Invoke(item);
            return item;
        }

        /// <summary>
        /// Returns an instance back to the pool. If the pool is at capacity the instance is destroyed instead.
        /// </summary>
        public void Release(T item)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (_collectionCheck && _stack.Contains(item))
            {
                Debug.LogError($"[ObjectPool] Double-release detected: {item} is already in the pool.");
                return;
            }

            _actionOnRelease?.Invoke(item);

            if (_stack.Count < _maxSize)
            {
                _stack.Push(item);
            }
            else
            {
                _actionOnDestroy?.Invoke(item);
                _countAll--;
            }
        }

        /// <summary>
        /// Destroys all inactive pooled instances and resets the count.
        /// Active instances are not affected.
        /// </summary>
        public void Clear()
        {
            while (_stack.Count > 0)
                _actionOnDestroy?.Invoke(_stack.Pop());

            _countAll = CountActive;
        }

        /// <summary>Clears the pool and marks it as disposed.</summary>
        public void Dispose()
        {
            if (_isDisposed) return;
            Clear();
            _isDisposed = true;
        }
    }
}
