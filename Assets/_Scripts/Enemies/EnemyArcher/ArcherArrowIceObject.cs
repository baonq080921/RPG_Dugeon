using System;
using System.Collections;
using UnityEngine;

public class ArcherArrowIceObject : MonoBehaviour
{
    [SerializeField] private Collider2D _col;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LayerMask _whatisTarget;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _damageScale = 1.6f;
    [SerializeField] private float _lifetime = 6f;

    private EntityCombat _combat;
    private Action _returnToPool;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetPool(Action returnToPool) => _returnToPool = returnToPool;

    public void SetUpArrow(float xVelocity, float direction, EntityCombat combat)
    {
        float flip = direction >= 0 ? 0 : 180;
        transform.Rotate(new Vector3(0, flip, 0));
        _rb.velocity = new Vector2(xVelocity * direction, 0);
        _combat = combat;
        StartCoroutine(LifetimeCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & _whatisTarget) != 0)
        {
            _combat.PerformedAttackOnTarget(collision.transform, _damageScale);
            StuckIntoTarget(collision.transform);
        }
    }

    private void StuckIntoTarget(Transform target)
    {
        StopAllCoroutines();
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        _col.enabled = false;
        transform.SetParent(target);
        transform.localPosition = Vector2.zero;
        _animator.SetBool("isExplose", true);
        StartCoroutine(ReleaseAfterDelay());
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(_lifetime);
        _returnToPool?.Invoke();
    }

    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(2f);
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
        _combat = null;
    }
}
