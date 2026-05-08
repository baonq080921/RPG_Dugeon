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
      var helperObject = new GameObject(nameof(Helper));
      helperObject.AddComponent<Helper>();
      Object.DontDestroyOnLoad(helperObject);
    }
  }
}
