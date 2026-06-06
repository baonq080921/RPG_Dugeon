using UnityEngine;

namespace scene
{
    /// <summary>
    /// Marks where the player spawns when arriving from a portal.
    /// The <see cref="PortalId"/> must match the originating portal's destination ID.
    /// </summary>
    public class SceneSpawnPoint : MonoBehaviour
    {
        [SerializeField] private string _portalId;

        /// <summary>Must match the portal's destination portal ID.</summary>
        public string PortalId => _portalId;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.6f, $"Spawn: {_portalId}");
#endif
        }
    }
}
