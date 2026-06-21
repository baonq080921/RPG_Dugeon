using Base;
using UnityEngine;

public class PoolManager : MonoBehaviour 
{

    public AfterImageEffect afterEffectPool;

    public ItemPickablePool itemObjectPool;
    
    public LevelUpEffectPool levelupPool;
    public HitEffectPool hitEffectPool;
    public SliceEffectPool sliceEffectPool;
    private void Awake()
    {
        ServiceLocator.Register<PoolManager>(this);
    }
}