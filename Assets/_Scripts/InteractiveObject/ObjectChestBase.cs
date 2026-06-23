using System;
using Interfaces;
using UnityEngine;

/// <summary>Base class for all interactable chest objects in the scene.</summary>
public class ObjectChestBase : MonoBehaviour, IHit, IScenePersistable
{
    [SerializeField] protected float currentHealth = 1f;
    protected Collider2D col2D;
    [SerializeField] protected Animator _animator;

    /// <summary>Scene-unique identifier generated automatically per scene instance. Never changes after first assignment.</summary>
    [SerializeField] private string _sceneEntityId;
    public string SceneEntityId => _sceneEntityId;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this)) return;
        if (string.IsNullOrEmpty(_sceneEntityId))
        {
            _sceneEntityId = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    [ContextMenu("Regenerate SceneEntityId")]
    private void RegenerateSceneEntityId()
    {
        _sceneEntityId = Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[ObjectChestBase] New SceneEntityId on '{name}': {_sceneEntityId}");
    }
#endif

    /// <inheritdoc/>
    public event Action OnPersisted;

    /// <summary>Invokes <see cref="OnPersisted"/>. Call from subclasses when the chest's persistent state changes.</summary>
    protected void RaiseOnPersisted() => OnPersisted?.Invoke();

    protected virtual void Awake()
    {
        col2D = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform target)
    {
        currentHealth -= damage + elementalDamage;
        if (currentHealth <= 0)
        {
            col2D.enabled = false;
            _animator.SetBool("Open", true);
            DropChestItem();
            RaiseOnPersisted();
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public void RestoreState()
    {
        col2D.enabled = false;
        _animator.SetBool("Open", true);
    }

    protected virtual void DropChestItem()
    {
    }
}