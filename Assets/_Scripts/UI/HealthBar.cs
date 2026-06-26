using enemy;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Keeps the health bar fill visually correct when the owning entity flips direction.
    /// Attach to the Canvas child of any Entity prefab (player or enemy).
    /// Counter-flips localScale so world scale stays +1, then syncs Slider.direction
    /// because UGUI fill is calculated in canvas-local space — a negative localScale.x
    /// inverts the local x-axis, making fill appear as (1 - value). Toggling direction corrects it.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        private Enemy _enemy;
        [SerializeField] private Slider _slider;

        void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            _slider.navigation = new Navigation { mode = Navigation.Mode.None };
            _slider.interactable = false;
        }

        void OnEnable()
        {
            _enemy.OnFlip += HealthBarFlip;
        }

        void OnDisable()
        {
            _enemy.OnFlip -= HealthBarFlip;
        }

        private void HealthBarFlip()
        {
            var s = transform.localScale;
            s.x = _enemy.direction * Mathf.Abs(s.x);
            transform.localScale = s;
            _slider.direction = s.x < 0f
                ? Slider.Direction.RightToLeft
                : Slider.Direction.LeftToRight;
        }
    }
}
