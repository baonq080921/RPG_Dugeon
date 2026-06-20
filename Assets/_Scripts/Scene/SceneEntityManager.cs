using System.Linq;
using Base;
using Interfaces;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace scene
{
    /// <summary>
    /// Per-scene manager that restores persistent entity state (killed enemies, opened chests, collected items, etc.) on load.
    /// Automatically discovers every <see cref="IScenePersistable"/> in the scene — no manual wiring needed
    /// when new persistable entity types are added.
    /// </summary>
    public class SceneEntityManager : MonoBehaviour
    {
        private IScenePersistable[] _persistables;
        private string _sceneName;
        private SaveManager _saveManager;

        private void Awake()
        {
            _sceneName = SceneManager.GetActiveScene().name;
        }

        private void Start()
        {
            _saveManager = ServiceLocator.Get<SaveManager>();
            _persistables = FindObjectsOfType<MonoBehaviour>()
                .OfType<IScenePersistable>()
                .ToArray();

            Debug.Log($"[SceneEntityManager] {_sceneName}: found {_persistables.Length} persistables: {string.Join(", ", System.Array.ConvertAll(_persistables, p => $"{p.GetType().Name}(id={p.SceneEntityId})"))}");

            SubscribeToEntities();
            ApplySceneState();
        }

        /// <summary>
        /// Restores all entities already marked as persisted in the saved scene state.
        /// Called from <see cref="Start"/> for portal transitions and directly by
        /// <see cref="SaveManager"/> after a full load to fix the timing gap.
        /// </summary>
        public void ApplySceneState()
        {
            if (_persistables == null) return;
            var sceneState = _saveManager?.GetSceneState(_sceneName);
            Debug.Log($"[SceneEntityManager] ApplySceneState on {_sceneName}: sceneState={(sceneState == null ? "NULL" : string.Join(", ", sceneState.persistedEntityIds))}");
            if (sceneState == null) return;

            foreach (var persistable in _persistables)
            {
                if (persistable == null) continue;
                bool found = sceneState.persistedEntityIds.Contains(persistable.SceneEntityId);
                Debug.Log($"[SceneEntityManager]   {persistable.GetType().Name} id={persistable.SceneEntityId} → restore={found}");
                if (found)
                    persistable.RestoreState();
            }
        }

        private void SubscribeToEntities()
        {
            foreach (var persistable in _persistables)
            {
                if (persistable == null) continue;
                string id = persistable.SceneEntityId;
                persistable.OnPersisted += () =>
                {
                    Debug.Log($"[SceneEntityManager] OnPersisted fired: {persistable.GetType().Name} id={id} in {_sceneName}");
                    _saveManager?.MarkEntityPersisted(_sceneName, id);
                };
            }
        }
    }
}
