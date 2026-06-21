using Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Displays the player's XP progress toward the next level.
    /// Assign a Slider for the fill bar and optionally a TextMeshProUGUI for the level number.
    /// </summary>
    public class UIXPBar : MonoBehaviour
    {
        [SerializeField] private Slider _xpSlider;
        [SerializeField] private TextMeshProUGUI _levelText;

        private EventBinding<PlayerXPChangedEvent> _xpChangedBinding;
        void Awake()
        {
            _xpSlider.interactable = false;
            _xpSlider.navigation = new Navigation{mode = Navigation.Mode.None};
            
        }

        private void OnEnable()
        {
            _xpChangedBinding = new EventBinding<PlayerXPChangedEvent>(OnXPChanged);
            EventBus<PlayerXPChangedEvent>.Register(_xpChangedBinding);
        }

        private void OnDisable()
        {
            EventBus<PlayerXPChangedEvent>.Deregister(_xpChangedBinding);
        }

        private void OnXPChanged(PlayerXPChangedEvent e)
        {
            _xpSlider.value = e.ExpToNextLevel > 0f ? e.CurrentExp / e.ExpToNextLevel : 0f;

            if (_levelText != null)
                _levelText.text = $"Lv.{e.Level}";
        }
    }
}
