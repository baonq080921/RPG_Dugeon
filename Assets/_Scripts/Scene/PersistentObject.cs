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
            if(instance == null)
                instance = this;
            else
                DontDestroyOnLoad(gameObject);
        }
    }
}
