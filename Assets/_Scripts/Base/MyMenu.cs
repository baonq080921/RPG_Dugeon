using Base;
using UnityEditor;
using UnityEngine;
using Base;
public static class MyMenu
{
    [MenuItem("Tools/MyTools/Reset All Player Stats")]
    private static void ResetStats()
    {
        EventBus<ResetStats>.Raise(new ResetStats());
    }
}