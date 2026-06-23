using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
namespace NPC
{
    public  class Npc : MonoBehaviour, IInteractable,IScenePersistable
    {

        [field:TextArea]
        [field:SerializeField] protected string _dialougeStr;

        public string SceneEntityId => _sceneEntityId;
        [SerializeField] private string _sceneEntityId;
        public  event Action OnPersisted;


    #if UNITY_EDITOR
        private void OnValidate()
        {
            // Skip the prefab asset itself — only assign IDs to scene instances so every
            // placed enemy gets its own GUID rather than inheriting one from the prefab.
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this)) return;
            if (string.IsNullOrEmpty(_sceneEntityId))
            {
                _sceneEntityId = Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        [UnityEditor.ContextMenu("Regenerate SceneEntityId")]
        private void RegenerateSceneEntityId()
        {
            _sceneEntityId = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"[Npc] New SceneEntityId on '{name}': {_sceneEntityId}");
        }
#endif


        // [SerializeField] protected UIFloating_Panel _uIFloating_Panel;
        public virtual void OnInteract()
        {
            // _uIFloating_Panel.ShowPanel(true);

        }

        public void RestoreState()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<player.PlayerInteract>(out var playerInteract))
            {
                playerInteract?.InteractDectect();
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            // _uIFloating_Panel.ShowPanel(false);
        }

        protected void RaiseOnPersistent()=> OnPersisted?.Invoke();

    }

}


