using System.Threading.Tasks;
using Interfaces;
using UnityEngine;

public class DemonEffect : MonoBehaviour 
{
    private Rigidbody2D _rb;
    private Collider2D _col;
    [SerializeField] private LayerMask _whatisTarget;
    [SerializeField] private float _flySpeed;
    private float _skillDamageMagic;
    private ElementType _elementType;
    private Animator _animator; 
    

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col =GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
    }


    public void SetUpDemonEffect(float magicDamge, float flyDirection, float scaleFactor, ElementType elementType)
    {
        _skillDamageMagic = magicDamge * scaleFactor;
        _rb.velocity = new Vector2(_flySpeed * flyDirection, _rb.velocity.y);
        _elementType = elementType;
        Flip(flyDirection);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(((1 << collision.gameObject.layer) & _whatisTarget) != 0)
        {
            Transform target = collision.GetComponent<Transform>();
            var targetVFx = target.GetComponent<EntityVfx>();
            if(collision.TryGetComponent<IHit>(out var hit))
            {
                hit?.TakeDamage(0,_skillDamageMagic,ElementType.None,target);
                targetVFx.UpdateStatusEffectVFX(_elementType, 0.3f);
                _rb.velocity = Vector2.zero;
                _animator.SetBool("CanExplose", true);
                _col.enabled = false;
                _rb.isKinematic = false;


                float exploseLength = _animator.GetCurrentAnimatorStateInfo(0).length;
                Destroy(gameObject,exploseLength);
            }
        }
    }

    private void Flip(float direction)
    {
        float angleFlip = direction > 0 ? 0 : 180;
        transform.Rotate(new Vector3(0,angleFlip, transform.position.z));
    }
}