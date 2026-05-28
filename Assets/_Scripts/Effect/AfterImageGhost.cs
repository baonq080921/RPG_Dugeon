using System;
using DG.Tweening;
using UnityEngine;

namespace player
{
    /// <summary>
    /// A single pooled ghost image. Fades out then invokes a callback so the caller can return it to the pool.
    /// Managed exclusively by <see cref="AfterImageEffect"/>.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class AfterImageGhost : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Tween _fadeTween;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Configures and activates the ghost for one fade-out cycle.
        /// </summary>
        /// <param name="sprite">Sprite frame to display.</param>
        /// <param name="sortingLayerID">Sorting layer copied from the source renderer.</param>
        /// <param name="sortingOrder">Sorting order — ghost renders one below the source.</param>
        /// <param name="flipX">Horizontal flip state copied from the source renderer.</param>
        /// <param name="startColor">Initial tint color (alpha drives fade start point).</param>
        /// <param name="duration">Seconds to fade from <paramref name="startColor"/> to transparent.</param>
        /// <param name="onComplete">Invoked when the fade finishes; pass back to pool here.</param>
        public void Play(
            Sprite sprite,
            int sortingLayerID,
            int sortingOrder,
            bool flipX,
            Color startColor,
            float duration,
            Action<AfterImageGhost> onComplete)
        {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingLayerID = sortingLayerID;
            _spriteRenderer.sortingOrder = sortingOrder;
            _spriteRenderer.flipX = flipX;
            _spriteRenderer.color = startColor;

            _fadeTween?.Kill();
            _fadeTween = _spriteRenderer.DOFade(0f, duration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => onComplete(this));
        }

        /// <summary>
        /// Kills any running tween and clears visual state so the ghost is safe for pool reuse.
        /// </summary>
        public void ResetState()
        {
            _fadeTween?.Kill();
            _fadeTween = null;
            _spriteRenderer.color = Color.clear;
        }
    }
}
