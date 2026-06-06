using System;
using Base;
using UnityEngine;

/// <summary>
/// Object pool for <see cref="SkillObjectTimeEcho"/> clones.
/// Attach this component alongside <see cref="SkillTimeEcho"/> on the same GameObject.
/// </summary>
public class SkillObjectTimeEchoPool : MonoBehaviourPool<SkillObjectTimeEcho>
{
    [SerializeField] private SkillObjectTimeEcho _prefab;

    private static Transform s_container;

    /// <inheritdoc/>
    protected override SkillObjectTimeEcho CreateInstance()
    {
        if (s_container == null)
        {
            s_container = new GameObject("[SkillObjectTimeEchoPool]").transform;
            DontDestroyOnLoad(s_container.gameObject);
        }
        var instance = Instantiate(_prefab, s_container);
        instance.gameObject.SetActive(false);
        return instance;
    }

    /// <inheritdoc/>
    protected override void OnRelease(SkillObjectTimeEcho instance) => instance.ResetObject();

    /// <summary>
    /// Gets an active clone from the pool with its release callback wired up.
    /// </summary>
    public SkillObjectTimeEcho GetInstance()
    {
        var instance = Get();
        instance.Init(() => Release(instance));
        return instance;
    }
}
