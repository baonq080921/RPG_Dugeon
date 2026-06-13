using Base;
using player;
using UnityEngine;

public class PoolManager : MonoBehaviour 
{

    public AfterImageEffect afterEffectPool;

    public ItemPickablePool itemObjectPool;
    
    public LevelUpEffectPool levelupPool;
    public HitEffectPool hitEffectPool;

    private void Awake()
    {
        ServiceLocator.Register<PoolManager>(this);
    }
}