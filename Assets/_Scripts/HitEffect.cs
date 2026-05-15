using UnityEngine;
public class HitEffect : MonoBehaviour 
{
    [SerializeField] private Animator _animator;
    public SpriteRenderer sr;
    public void EnableHit()
    {
        _animator.SetBool("OnHit",true);
    }

    public void DisableHit()
    {
        _animator.SetBool("OnHit",false);
    }


    
}