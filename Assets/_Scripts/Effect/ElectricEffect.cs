using System;
using Interfaces;
using UnityEngine;


namespace Effect
{
    /// <summary>
    /// One-shot electric passive VFX that damages every <see cref="IHit"/> inside its radius.
    /// The damage value is injected by the spawner (see <see cref="Damage"/>) so this effect
    /// never reaches into the player layer — keeping the Effect package dependency-free of game entities.
    /// </summary>
    public class ElectricEffect : MonoBehaviour
    {

        [SerializeField] private float _damageRadius;
        private Collider2D[] _results ;
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _isDraw;
        public Action OnComplete;

        /// <summary>Final elemental damage to deal on the next radius hit. Set by the spawner before the animation event fires.</summary>
        public float Damage { get; set; }

        void Awake()
        {
            _results = new Collider2D[20];
            _animator = GetComponent<Animator>();
        }

        /// <summary>Animation event: damages every <see cref="IHit"/> in range using the injected <see cref="Damage"/>.</summary>
        public void DamageTargetInRadius()
        {
            int dectects = Physics2D.OverlapCircleNonAlloc(transform.position,_damageRadius,_results);
            for(int i = 0 ; i < dectects; i++)
            {
                if(_results[i].TryGetComponent<IHit>(out var hit))
                {
                    hit?.TakeDamage(0, Damage, ElementType.Electric, transform);
                }
            }
        }

        public void ReturnToPool()
        {
            OnComplete?.Invoke();
        }

        public void ResetAnimation()
        {
            _animator.Rebind();
        }

        void OnDrawGizmos()
        {
            if(!_isDraw) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _damageRadius);
        }
    }

}
