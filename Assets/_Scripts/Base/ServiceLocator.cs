using System;
using System.Collections.Generic;
using UnityEngine;
namespace Base
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Register<T>(T service) => services[typeof(T)] = service;


        public static T Get<T>()
        {
            if (!services.TryGetValue(typeof(T), out var service)) return default;
            // Unity objects return true for == null when destroyed; treat them as missing.
            if (service is UnityEngine.Object unityObj && !unityObj) return default;
            return (T)service;
        }
    }
}