using Base;
using UnityEngine;

namespace Pool
{
    /// <summary>
    /// Manages two <see cref="ObjectPool{T}"/> instances — one for the default echo prefab
    /// and one for the Mahoraga variant — so callers never allocate via Instantiate at runtime.
    /// </summary>
    public class SkillObjectTimeEchoPool : MonoBehaviour
    {
        [SerializeField] private SkillObjectTimeEcho _defaultPrefab;
        [SerializeField] private SkillObjectTimeEcho _mahoragaPrefab;

        private ObjectPool<SkillObjectTimeEcho> _defaultPool;
        private ObjectPool<SkillObjectTimeEcho> _mahoragaPool;

        private void Awake()
        {
            _defaultPool = BuildPool(_defaultPrefab);
            _mahoragaPool = BuildPool(_mahoragaPrefab);
        }

        /// <summary>Returns an active default-variant echo instance from its pool.</summary>
        public SkillObjectTimeEcho GetDefault() => Rent(_defaultPool);

        /// <summary>Returns an active Mahoraga-variant echo instance from its pool.</summary>
        public SkillObjectTimeEcho GetMahoraga() => Rent(_mahoragaPool);

        private SkillObjectTimeEcho Rent(ObjectPool<SkillObjectTimeEcho> pool)
        {
            var instance = pool.Get();
            instance.Init(() => pool.Release(instance));
            return instance;
        }

        private ObjectPool<SkillObjectTimeEcho> BuildPool(SkillObjectTimeEcho prefab)
        {
            return new ObjectPool<SkillObjectTimeEcho>(
                createFunc: () => Create(prefab),
                actionOnGet: obj => obj.gameObject.SetActive(true),
                actionOnRelease: obj => { obj.ResetObject(); obj.gameObject.SetActive(false); },
                actionOnDestroy: obj => { if (obj != null) Destroy(obj.gameObject); }
            );
        }

        private SkillObjectTimeEcho Create(SkillObjectTimeEcho prefab)
        {
            var instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnDestroy()
        {
            _defaultPool?.Dispose();
            _mahoragaPool?.Dispose();
        }
    }
}
