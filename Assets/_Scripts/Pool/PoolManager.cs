using Base;
using UnityEngine;

namespace Pool
{
    public class PoolManager : MonoBehaviour
    {

        public AfterImageEffect afterEffectPool;

        public ItemPickablePool itemObjectPool;

        public LevelUpEffectPool levelupPool;
        public HitEffectPool hitEffectPool;
        public SliceEffectPool sliceEffectPool;
        public DamagePopupPool damagePopupPool;
        private void Awake()
        {
            ServiceLocator.Register<PoolManager>(this);
        }
    }
}
