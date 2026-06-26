using Base;
using Effect;
using UnityEngine;

namespace Pool
{
    /// <summary>
    /// Pool for <see cref="ElectricEffect"/> VFX instances.
    /// Assigned to <see cref="PoolManager"/> in the Inspector — do not register with ServiceLocator directly.
    /// </summary>
    public class ElectricEffectPool : MonoBehaviourPool<ElectricEffect>
    {
        [SerializeField] private ElectricEffect _prefab;

        protected override void Awake()
        {
            base.Awake();
            var manager = ServiceLocator.Get<PoolManager>();
            if (manager != null) manager.electricEffectPool = this;
        }

        protected override void OnDestroy()
        {
            var manager = ServiceLocator.Get<PoolManager>();
            if (manager != null && manager.electricEffectPool == this)
                manager.electricEffectPool = null;
        }


        protected override void OnGet(ElectricEffect item)
        {
            item.ResetAnimation();
            item.OnComplete = () => OnRelease(item);
        }

        protected override void OnRelease(ElectricEffect item)
        {
            item.OnComplete = null;
            item.transform.SetParent(null);
        }

        /// <summary>Spawns an electric effect at <paramref name="targetTf"/> that will deal <paramref name="damage"/> on hit.</summary>
        /// <param name="targetTf">Transform the effect attaches to and is positioned at.</param>
        /// <param name="damage">Final elemental damage the effect deals to targets in range.</param>
        public void Spawn(Transform targetTf, float damage)
        {
            var item = Get();
            item.Damage = damage;
            item.transform.position = targetTf.position;
            item.transform.SetParent(targetTf);
        }

        protected override ElectricEffect CreateInstance()
        {
            ElectricEffect electricEffect = Instantiate(_prefab);
            electricEffect.gameObject.SetActive(false);
            return electricEffect;
        }
    }
}
