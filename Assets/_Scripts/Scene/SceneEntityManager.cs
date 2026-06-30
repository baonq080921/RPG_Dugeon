using System.Linq;
using Base;
using Interfaces;
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
        private ISaveService _saveService;
        private EventBinding<SceneStateRestoredEvent> _sceneStateRestoredBinding;

        private void Awake()
        {
            _sceneName = SceneManager.GetActiveScene().name;
        }

        private void OnEnable()
        {
            _sceneStateRestoredBinding = new EventBinding<SceneStateRestoredEvent>(ApplySceneState);
            EventBus<SceneStateRestoredEvent>.Register(_sceneStateRestoredBinding);
        }

        private void OnDisable()
        {
            EventBus<SceneStateRestoredEvent>.Deregister(_sceneStateRestoredBinding);
        }

        private void Start()
        {
            _saveService = ServiceLocator.Get<ISaveService>();
            _persistables = FindObjectsOfType<MonoBehaviour>()
                .OfType<IScenePersistable>()
                .ToArray();

            Debug.Log($"[SceneEntityManager] {_sceneName}: found {_persistables.Length} persistables: {string.Join(", ", System.Array.ConvertAll(_persistables, p => $"{p.GetType().Name}(id={p.SceneEntityId})"))}");

            SubscribeToEntities();
            ApplySceneState();
        }

        private void ApplySceneState()
        {
            if (_persistables == null || _saveService == null) return;

            foreach (var persistable in _persistables)
            {
                if (persistable == null) continue;
                if (_saveService.IsEntityPersisted(_sceneName, persistable.SceneEntityId))
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
                    _saveService?.MarkEntityPersisted(_sceneName, id);
                };
            }
        }
    }
}
