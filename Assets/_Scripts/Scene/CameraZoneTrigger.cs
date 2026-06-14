using Base;
using UnityEngine;

namespace scene
{
    /// <summary>
    /// Place one per camera zone in the scene. When the player enters this trigger,
    /// the Cinemachine Confiner switches to this zone's bounding collider.
    /// Use this instead of a single CompositeCollider2D when rooms are not connected.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CameraZoneTrigger : MonoBehaviour
    {
        [Tooltip("The collider that defines camera bounds for this zone. Can be this object's own collider.")]
        [SerializeField] private Collider2D _boundingShape;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            if (_boundingShape == null)
                _boundingShape = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<player.Player>(out _)) return;
            EventBus<CameraZoneChangedEvent>.Raise(new CameraZoneChangedEvent(_boundingShape));
        }
    }
}
