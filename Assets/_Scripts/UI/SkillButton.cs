using player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// On-screen skill button for mobile. Calls <see cref="SkillManager.PressSkill"/> on tap.
    /// Set <see cref="_slotIndex"/> in the Inspector to match the desired skill slot (0-based).
    /// Automatically tracks whichever <see cref="Player"/> is currently active.
    /// </summary>
    public class SkillButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private ButtonSkillName _skillName;

        private Player _player;

        private void OnEnable()
        {
            Player.ActivePlayerChanged += OnActivePlayerChanged;
        }

        private void Start()
        {
            if (_player != null) return;
            var existing = FindObjectOfType<Player>();
            if (existing != null)
                OnActivePlayerChanged(existing);
        }

        private void OnDisable()
        {
            Player.ActivePlayerChanged -= OnActivePlayerChanged;
        }

        private void OnActivePlayerChanged(Player player)
        {
            _player = player;
        }

        /// <inheritdoc/>
        public void OnPointerDown(PointerEventData eventData)
        {
            _player?.SkillButtonHandler.PressSkill(_skillName);
        }
    }
}
