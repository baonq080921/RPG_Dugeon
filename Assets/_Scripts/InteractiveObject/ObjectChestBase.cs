using System;
using Interfaces;
using UnityEngine;

/// <summary>Base class for all interactable chest objects in the scene.</summary>
public class ObjectChestBase : MonoBehaviour, IHit
{
    [SerializeField] protected float currentHealth = 1f;
    [SerializeField] protected Collider2D collider2D;
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
#endif

    /// <summary>Raised once when this chest is opened. Subscribed to by <see cref="scene.SceneEntityManager"/>.</summary>
    public event Action OnOpened;

    protected virtual void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType elementType, Transform target)
    {
        currentHealth -= damage + elementalDamage;
        if (currentHealth <= 0)
        {
            collider2D.enabled = false;
            _animator.SetBool("Open", true);
            DropChestItem();
            OnOpened?.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>Restores the already-opened visual state without re-dropping items. Called by <see cref="scene.SceneEntityManager"/> on scene load.</summary>
    public void SetOpenedImmediately()
    {
        collider2D.enabled = false;
        _animator.SetBool("Open", true);
    }

    protected virtual void DropChestItem()
    {
    }
}