
using UnityEngine;


namespace Base
{
    public static class DebugCustom
    {
        public static void Log(string message)
        {
            Debug.Log($"[Custom Log]<color=green>{message}</color>");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[Custom Warning]<color=yellow>{message}</color>");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"[Custom Error]<color=red>{message}</color>");
        }
    }
}