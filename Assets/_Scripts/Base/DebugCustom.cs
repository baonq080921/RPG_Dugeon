
using UnityEngine;


namespace Base
{
    public static class DebugCustom
    {
        public static void Log(string message)
        {
            Debug.Log($"<color=green>[Custom Log]</color> <color=yellow>{message}</color>");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"<color=yellow>[Custom Warning]</color> <color=white>{message}</color>");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"<color=red>[Custom Error]</color> <color=white>{message}</color>");
        }
    }
}