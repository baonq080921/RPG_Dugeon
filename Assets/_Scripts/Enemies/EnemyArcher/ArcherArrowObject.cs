using System;
using System.Collections;
using entity;
using Interfaces;
using UnityEngine;

namespace enemy
{
    public class ArcherArrowObject : MonoBehaviour, ICounterable
    {
        [SerializeField] private Collider2D _col;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private LayerMask _whatisTarget;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _lifetime = 6f;

        private EntityCombat _combat;
        private Action _returnToPool;
        private LayerMask _originalWhatisTarget;

        public bool CanCounter { get; set; }

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
            _originalWhatisTarget = _whatisTarget;
        }

        public void SetPool(Action returnToPool) => _returnToPool = returnToPool;

        public void SetUpArrow(float xVelocity, float direction, EntityCombat combat)
        {
            float flip = direction > 0 ? 0 : 180;
            CanCounter = true;
            transform.Rotate(new Vector3(0, flip, 0));
            _rb.velocity = new Vector2(xVelocity * direction, 0);
            _combat = combat;
            StartCoroutine(LifetimeCoroutine());
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & _whatisTarget) != 0)
            {
                _combat.PerformedAttackOnTarget(collision.transform);
                StuckIntoTarget(collision.transform);
            }
        }

        private void StuckIntoTarget(Transform target)
        {
            StopAllCoroutines();
            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true;
            _col.enabled = false;
            _animator.enabled = false;
            transform.SetParent(target);
            StartCoroutine(ReleaseAfterDelay());
        }

        private IEnumerator LifetimeCoroutine()
        {
            yield return new WaitForSeconds(_lifetime);
            _returnToPool?.Invoke();
        }

        private IEnumerator ReleaseAfterDelay()
        {
            yield return new WaitForSeconds(1.5f);
            _returnToPool?.Invoke();
        }

        public void ResetArrow()
        {
            StopAllCoroutines();
            _rb.velocity = Vector2.zero;
            _rb.isKinematic = false;
            _col.enabled = true;
            _animator.enabled = true;
            _animator.Rebind();
            transform.SetParent(null);
            transform.rotation = Quaternion.identity;
            CanCounter = false;
            _whatisTarget = _originalWhatisTarget;
            _combat = null;
        }

        public void HandleCounter()
        {
            _rb.velocity = new Vector2(_rb.velocity.x * -1, _rb.velocity.y);
            transform.Rotate(new Vector3(0, 180, 0));
            _whatisTarget |= 1 << LayerMask.NameToLayer("Enemy");
        }

        public void EnableCounter() { }
        public void DisableCounter() { }
    }
}
