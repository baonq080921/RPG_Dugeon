using UnityEngine;

namespace scene
{
    /// <summary>
    /// Marker placed on the bounding collider in each scene.
    /// <see cref="CameraResolutionFitter"/> finds this on scene load and assigns
    /// the attached <see cref="Collider2D"/> to the Cinemachine Confiner.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CameraConfinerBounds : MonoBehaviour
    {
        public Collider2D BoundingShape => GetComponent<Collider2D>();
    }
}
