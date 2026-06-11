using Base;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
namespace System
{
    public static class MyMenu
    {
        [MenuItem("Tools/MyTools/Reset All Player Stats")]
        public static void ResetStats()
        {
            DebugCustom.Log("All Stats Restart to base Value");
            EventBus<ResetStats>.Raise(new ResetStats());
        }
    }
}
