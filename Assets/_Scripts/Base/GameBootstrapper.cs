using UnityEngine;

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
      var persistentPrefab = Resources.Load("PERSISTENTOBJECT");
      if (persistentPrefab != null)
          Object.DontDestroyOnLoad(Object.Instantiate(persistentPrefab));
      else
          Debug.LogError("[GameBootstrapper] PERSISTENTOBJECT prefab not found in Resources!");
      var helperObject = new GameObject(nameof(Helper));
      helperObject.AddComponent<Helper>();
      Object.DontDestroyOnLoad(helperObject);

      var cameraFitterObject = new GameObject(nameof(CameraResolutionFitter));
      cameraFitterObject.AddComponent<CameraResolutionFitter>();
      Object.DontDestroyOnLoad(cameraFitterObject);

    }
  }
}
