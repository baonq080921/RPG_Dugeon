using System;
using System.Collections.Generic;
using UnityEngine;
namespace Base
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Register<T>(T service) => services[typeof(T)] = service;


        public static T Get<T>() => services.TryGetValue(typeof(T), out var service) ? (T)service : default;
    }
}