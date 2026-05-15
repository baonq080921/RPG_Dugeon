using UnityEngine;
namespace Stats
{
    [System.Serializable]
    public class Stat
    {
        [SerializeField]private float baseValue;
        public float GetValue() => baseValue;

    }
}