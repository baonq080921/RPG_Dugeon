using Base;
using UnityEngine;

namespace scene
{
    /// <summary>
    /// Place in any scene. When the player walks into this trigger,
    /// it loads <see cref="_targetScene"/> and spawns the player at the
    /// <see cref="SceneSpawnPoint"/> whose ID matches <see cref="_destinationPortalId"/>.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Portal : MonoBehaviour
    {
        [SerializeField] private string _targetScene;
        [SerializeField] private string _destinationPortalId;
        [Tooltip("If true, the player must complete this scene's quest before passing through.")]
        [SerializeField] private bool _requireQuestCompletion = false;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (SceneTransitionManager.Instance == null) return;
            if (SceneTransitionManager.Instance.IsTransitioning) return;
            if (!other.TryGetComponent<player.Player>(out _)) return;

            if (_requireQuestCompletion)
            {
                var questController = FindObjectOfType<Quest.SceneQuestController>();
                if (questController != null && !questController.IsQuestComplete)
                {
                    EventBus<Base.AlertNotiEvent>.Raise(new Base.AlertNotiEvent($"Finish the quest first!\n{questController.Quest.QuestName}"));
                    return;
                }
            }

            SceneTransitionManager.Instance.TransitionToScene(_targetScene, _destinationPortalId);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 1f, 0.5f, 0.25f);
            Gizmos.DrawCube(transform.position, transform.localScale);
            Gizmos.color = new Color(0.2f, 1f, 0.5f, 1f);
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}
