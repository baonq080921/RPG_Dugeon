using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Switches between the Skill Tree and Inventory canvases using DOTween slide animations
    /// and highlights the active tab button.
    /// </summary>
    public class UICanvasChange : MonoBehaviour
    {
        [SerializeField] private Button _btnToSkillTree;
        [SerializeField] private Button _btnToInventory;

        [SerializeField] private Canvas _skillTreeCanvas;
        [SerializeField] private Canvas _inventoryCanvas;

        [SerializeField] private RectTransform _skillTreeRect;
        [SerializeField] private RectTransform _inventoryRect;

        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _normalColor = Color.grey;

        [SerializeField] private float _tweenDuration = 0.3f;
        // Horizontal distance to slide off-screen; set to match your canvas/panel width.
        [SerializeField] private float _slideDistance = 1080f;

        private Vector2 _skillTreeOriginalPosition;
        private Vector2 _inventoryOriginalPosition;

        private Canvas _activeCanvas;

        void Awake()
        {
            _skillTreeOriginalPosition = _skillTreeRect.anchoredPosition;
            _inventoryOriginalPosition = _inventoryRect.anchoredPosition;

            _btnToSkillTree.onClick.AddListener(() => ShowCanvas(_skillTreeCanvas));
            _btnToInventory.onClick.AddListener(() => ShowCanvas(_inventoryCanvas));
        }

        void Start()
        {
            // Place skill tree off-screen immediately so it doesn't flash.
            _skillTreeRect.anchoredPosition = _skillTreeOriginalPosition + new Vector2(_slideDistance*10f, 0f);

            _activeCanvas = _inventoryCanvas;
            _btnToSkillTree.image.color = _normalColor;
            _btnToInventory.image.color = _selectedColor;
        }

        /// <summary>
        /// Slides in the requested canvas from the right and slides the current one out to the left.
        /// </summary>
        public void ShowCanvas(Canvas canvasShow)
        {
            if (canvasShow == _activeCanvas) return;

            bool showSkill = canvasShow == _skillTreeCanvas;

            RectTransform incoming = showSkill ? _skillTreeRect : _inventoryRect;
            RectTransform outgoing = showSkill ? _inventoryRect : _skillTreeRect;
            Vector2 incomingTarget = showSkill ? _skillTreeOriginalPosition : _inventoryOriginalPosition;
            Vector2 outgoingTarget = showSkill ? _inventoryOriginalPosition : _skillTreeOriginalPosition;

            incoming.DOKill();
            outgoing.DOKill();

            // Snap incoming to the right edge, then slide it in.
            incoming.anchoredPosition = incomingTarget + new Vector2(_slideDistance, 0f);
            incoming.DOAnchorPos(incomingTarget, _tweenDuration).SetUpdate(true).SetEase(Ease.OutCubic);

            // Slide outgoing to the left edge.
            outgoing.DOAnchorPos(outgoingTarget + new Vector2(-_slideDistance*10f, 0f), _tweenDuration).SetUpdate(true)
                    .SetEase(Ease.InCubic);

            _btnToSkillTree.image.color = showSkill ? _selectedColor : _normalColor;
            _btnToInventory.image.color = showSkill ? _normalColor : _selectedColor;

            _activeCanvas = canvasShow;
        }
    }
}
