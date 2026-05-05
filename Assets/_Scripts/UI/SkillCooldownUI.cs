using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Generic skill cooldown overlay. Attach to any skill button, assign the fill
    /// image and timer text, then call <see cref="StartCooldown"/> to run the effect.
    /// </summary>
    public class 
    
    SkillCooldownUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _timerText;

        private Coroutine _cooldownCoroutine;

        private void Awake()
        {
            SetOverlayActive(false);
        }

        /// <summary>
        /// Starts the cooldown visual sweep for the given duration.
        /// Safe to call while a cooldown is already running — it restarts.
        /// </summary>
        /// <param name="duration">Total cooldown time in seconds.</param>
        public void StartCooldown(float duration)
        {
            if (_cooldownCoroutine != null)
                StopCoroutine(_cooldownCoroutine);
            _cooldownCoroutine = StartCoroutine(RunCooldown(duration));
        }

        private IEnumerator RunCooldown(float duration)
        {
            SetOverlayActive(true);
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var remaining = Mathf.Max(0f, duration - elapsed);
                _fillImage.fillAmount = remaining / duration;
                _timerText.text = remaining > 0.05f ? remaining.ToString("F1") : string.Empty;
                yield return null;
            }

            _fillImage.fillAmount = 0f;
            _timerText.text = string.Empty;
            SetOverlayActive(false);
            _cooldownCoroutine = null;
        }

        private void SetOverlayActive(bool isActive)
        {
            _fillImage.gameObject.SetActive(isActive);
        }
    }
}
