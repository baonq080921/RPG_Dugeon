using System.Collections.Generic;
using UnityEngine;

namespace scene
{
    /// <summary>
    /// Marks this GameObject as persistent across scene loads.
    /// If a persisted object with the same name already exists, the new duplicate is destroyed.
    /// Attach to: PlayerYuji, Camera, Canvas, EventSystem, HitEffectPool.
    /// </summary>
    public class PersistentObject : MonoBehaviour
    {
        private static readonly Dictionary<string, GameObject> s_persistedObjects = new();

        private void Awake()
        {
            string key = gameObject.name;
            if (s_persistedObjects.TryGetValue(key, out var existing) && existing != null)
            {
                // Disable all behaviours before OnEnable fires so components like
                // EventSystem don't log warnings during the one frame before Destroy takes effect.
                foreach (var behaviour in GetComponents<Behaviour>())
                {
                    if (behaviour != this) behaviour.enabled = false;
                }
                Destroy(gameObject);
                return;
            }
            s_persistedObjects[key] = gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }
}
