using System.Threading.Tasks;
using Base;
using Effect;
using UnityEngine;

namespace Pool
{
    /// <summary>
    /// Manages two separate object pools for normal and critical <see cref="HitEffect"/> instances.
    /// Assigned to <see cref="PoolManager"/> in the Inspector — do not register with ServiceLocator directly.
    /// </summary>
    public class HitEffectPool : MonoBehaviourPool<HitEffect>
    {
        [SerializeField] private HitEffect _hitPrefab;
        [SerializeField] private HitEffect _hitCritPrefab;

        private ObjectPool<HitEffect> _critPool;

        protected override void Awake()
        {
            base.Awake();
            var manager = ServiceLocator.Get<PoolManager>();
            if(manager !=  null) manager.hitEffectPool = this;
            _critPool = new ObjectPool<HitEffect>(
                createFunc: () => { var h = Instantiate(_hitCritPrefab); h.gameObject.SetActive(false); return h; },
                actionOnGet: h => { h.gameObject.SetActive(true); h.EnableHit(); },
                actionOnRelease: h => { h.DisableHit(); h.transform.SetParent(null); h.gameObject.SetActive(false); },
                actionOnDestroy: h => { if (h != null) Destroy(h.gameObject); });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _critPool?.Dispose();
            var manager = ServiceLocator.Get<PoolManager>();
            if(manager != null && manager.hitEffectPool == this )
                manager.hitEffectPool = null;
        }

        protected override HitEffect CreateInstance()
        {
            var hit = Instantiate(_hitPrefab);
            hit.gameObject.SetActive(false);
            return hit;
        }

        protected override void OnGet(HitEffect hit) => hit.EnableHit();

        protected override void OnRelease(HitEffect hit)
        {
            hit.DisableHit();
            hit.ResetColor();
            hit.transform.SetParent(null);
        }

        /// <summary>
        /// Spawns a normal or critical hit effect at <paramref name="targetTf"/> and returns it to the pool after 1 second.
        /// </summary>
        public void SpawnHitEffect(Transform targetTf, Color hitColor, Vector2 randomHitOffSet, bool isCrit)
        {
            var hit = isCrit ? _critPool.Get() : Get();
            hit.transform.SetParent(targetTf);
            if (!isCrit)
            {
                hit.SetRandomPositionVFX(randomHitOffSet, hit);
                hit.SetRandomRotation();
                hit.sr.color = hitColor;
            }
            else
            {
                hit.transform.localPosition = Vector3.zero;
            }
            ReturnAfterDelay(hit, isCrit);
        }

        private async void ReturnAfterDelay(HitEffect hit, bool isCrit)
        {
            await Task.Delay(1000);
            if (hit == null) return;
            if (isCrit)
                _critPool.Release(hit);
            else
                Release(hit);
        }
    }
}
