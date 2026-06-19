using Base;
using enemy;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace scene
{
    /// <summary>
    /// Per-scene manager that tracks which enemies have been killed and which chests have been opened.
    /// On scene load it disables already-dead enemies and restores the opened visual state of chests
    /// without re-dropping items. State is persisted to disk so Continue restores it; New Game clears it.
    /// </summary>
    public class SceneEntityManager : MonoBehaviour
    {
        [SerializeField] private Enemy[]           _enemies = System.Array.Empty<Enemy>();
        [SerializeField] private ObjectChestBase[] _chests  = System.Array.Empty<ObjectChestBase>();
        private string _sceneName;

        private void Awake()
        {
            _sceneName = SceneManager.GetActiveScene().name;
        }

        private void Start()
        {
            SubscribeToEntities();

            // Portal transitions already have _sceneStates populated before Start runs.
            // Full loads do not — SaveManager.ApplyData calls ApplySceneState() explicitly
            // after restoring _sceneStates, so this handles only the portal case.
            ApplySceneState();
        }

        /// <summary>
        /// Disables enemies and sets chests that are already marked in the saved scene state.
        /// Called from <see cref="Start"/> for portal transitions and directly by
        /// <see cref="SaveManager"/> after a full load to fix the timing gap.
        /// </summary>
        public void ApplySceneState()
        {
            var sceneState = ServiceLocator.Get<SaveManager>()?.GetSceneState(_sceneName);
            if (sceneState == null) return;

            foreach (var enemy in _enemies)
            {
                if (enemy == null) continue;
                if (sceneState.killedEnemyIds.Contains(enemy.SceneEntityId))
                    enemy.gameObject.SetActive(false);
            }

            foreach (var chest in _chests)
            {
                if (chest == null) continue;
                if (sceneState.openedChestIds.Contains(chest.SceneEntityId))
                    chest.SetOpenedImmediately();
            }
        }

        private void SubscribeToEntities()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy == null) continue;
                string id = enemy.SceneEntityId;
                enemy.OnDied += () => NotifyEnemyKilled(id);
            }

            foreach (var chest in _chests)
            {
                if (chest == null) continue;
                string id = chest.SceneEntityId;
                chest.OnOpened += () => NotifyChestOpened(id);
            }
        }

        private void NotifyEnemyKilled(string enemyId)
        {
            ServiceLocator.Get<SaveManager>()?.MarkEnemyKilled(_sceneName, enemyId);
        }

        private void NotifyChestOpened(string chestId)
        {
            ServiceLocator.Get<SaveManager>()?.MarkChestOpened(_sceneName, chestId);
        }
    }
}
