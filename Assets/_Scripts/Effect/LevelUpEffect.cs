using System;
using UnityEngine;

/// <summary>
/// VFX for player level-up. Calls <see cref="OnComplete"/> when the animation finishes
/// so the pool can reclaim the instance automatically.
/// </summary>
public class LevelUpEffect : MonoBehaviour
{
    public bool isDone { get; private set; }

    /// <summary>Set by the pool before the effect is activated. Invoked when the animation ends.</summary>
    public Action OnComplete;

    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    /// <summary>Called by the Animator Event at the last frame of the effect clip.</summary>
    public void SetAnimation()
    {
        isDone = true;
        _animator.SetBool("isDone", true);
        OnComplete?.Invoke();
    }

    /// <summary>Resets state and restarts the animation from frame 0 for pool reuse.</summary>
    public void ResetAnimation()
    {
        isDone = false;
        _animator.SetBool("isDone", false);
        _animator.Rebind();
        _animator.Update(0f);
    }
}
