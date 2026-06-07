using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
  /// <summary>
  /// Automatically creates a Helper instance before any scene loads.
  /// </summary>
  public static class GameBootstrapper
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
      var helperObject = new GameObject(nameof(Helper));
      helperObject.AddComponent<Helper>();
      Object.DontDestroyOnLoad(helperObject);

      var cameraFitterObject = new GameObject(nameof(CameraResolutionFitter));
      cameraFitterObject.AddComponent<CameraResolutionFitter>();
      Object.DontDestroyOnLoad(cameraFitterObject);

      SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // Clear all EventBus channels on scene unload so stale MonoBehaviour
    // subscribers from the previous scene cannot fire into the new one.
    private static void OnSceneUnloaded(Scene scene)
    {
      EventBus<TargetGotHitEvent>.Clear();
      EventBus<PlayerAddHealthAmount>.Clear();
      EventBus<PlayerDiedEvent>.Clear();
      EventBus<EnemyDiedEvent>.Clear();
    }
  }
}
