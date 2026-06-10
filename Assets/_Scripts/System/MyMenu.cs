using Base;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
namespace System
{
    public static class MyMenu
    {
        [MenuItem("Tools/MyTools/Reset All Player Stats")]
        private static void ResetStats()
        {
            EventBus<ResetStats>.Raise(new ResetStats());
        }
    }
}
