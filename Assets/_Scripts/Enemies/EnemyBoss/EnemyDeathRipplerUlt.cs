using Interfaces;
using UnityEngine;

public class EnemyDeathRipplerUlt : MonoBehaviour
{
    [SerializeField] private Collider2D _collider2D;
    private float _ultDamage;
    private float _elementalUltDamage;
    void Awake()
    {

        _collider2D = GetComponent<Collider2D>();
        _collider2D.enabled = false;
    }
    public void EnableCollider() => _collider2D.enabled = true;
    public void DisableCollider() => _collider2D.enabled = false;
    public void SetUpUltimateSkill(float ultDamage, float elementalDamage, float scaleFactor = 1f)
    {
        _ultDamage = ultDamage * scaleFactor;
        _elementalUltDamage = elementalDamage * scaleFactor;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IHit>(out var hit))
        {
            hit?.TakeDamage(_ultDamage,_elementalUltDamage,ElementType.None,null);
        }
    }

}