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
        public static PersistentObject instance;

        private void Awake()
        {
            // Persistence is handled at the prefab root by GameBootstrapper (Object.DontDestroyOnLoad
            // on the instantiated PERSISTENTOBJECT root), so children are kept automatically.
            // This component only guards against duplicates; it must NOT call DontDestroyOnLoad
            // itself because it lives on a child GameObject (Unity ignores it and warns).
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
