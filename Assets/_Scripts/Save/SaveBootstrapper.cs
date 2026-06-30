using UnityEngine;

namespace Save
{
    /// <summary>
    /// Creates the <see cref="SaveManager"/> before any scene loads so save data is always available.
    /// Separated from GameBootstrapper so Base does not depend on the Save layer.
    /// </summary>
    public static class SaveBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var saveManagerObject = new GameObject(nameof(SaveManager));
            saveManagerObject.AddComponent<SaveManager>();
            Object.DontDestroyOnLoad(saveManagerObject);
        }
    }
}
