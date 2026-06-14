using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Switches between the Skill Tree, Inventory, and Quest canvases using DOTween slide animations
    /// and highlights the active tab button.
    /// </summary>
    public class UICanvasChange : MonoBehaviour
    {
        [SerializeField] private Button _btnToSkillTree;
        [SerializeField] private Button _btnToInventory;
        [SerializeField] private Button _btnToQuest;

        [SerializeField] private Canvas _skillTreeCanvas;
        [SerializeField] private Canvas _inventoryCanvas;
        [SerializeField] private Canvas _questCanvas;

        [SerializeField] private RectTransform _skillTreeRect;
        [SerializeField] private RectTransform _inventoryRect;
        [SerializeField] private RectTransform _questRect;

        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _normalColor = Color.grey;

        [SerializeField] private float _tweenDuration = 0.3f;
        [SerializeField] private float _slideDistance = 1080f;

        private Vector2 _skillTreeOriginalPos;
        private Vector2 _inventoryOriginalPos;
        private Vector2 _questOriginalPos;

        private Canvas _activeCanvas;

        void Awake()
        {
            _skillTreeOriginalPos = _skillTreeRect.anchoredPosition;
            _inventoryOriginalPos = _inventoryRect.anchoredPosition;
            if (_questRect != null)
                _questOriginalPos = _questRect.anchoredPosition;

            _btnToSkillTree.onClick.AddListener(() => ShowCanvas(_skillTreeCanvas));
            _btnToInventory.onClick.AddListener(() => ShowCanvas(_inventoryCanvas));
            if (_btnToQuest != null)
                _btnToQuest.onClick.AddListener(() => ShowCanvas(_questCanvas));
        }

        void Start()
        {
            SlideOffScreen(_skillTreeRect, _skillTreeOriginalPos);
            if (_questRect != null)
                SlideOffScreen(_questRect, _questOriginalPos);

            _activeCanvas = _inventoryCanvas;
            RefreshButtonColors(_inventoryCanvas);
        }

        /// <summary>Slides in the requested canvas and slides out the current one.</summary>
        public void ShowCanvas(Canvas canvasToShow)
        {
            if (canvasToShow == _activeCanvas) return;

            SlideOut(_activeCanvas);
            SlideIn(canvasToShow);

            _activeCanvas = canvasToShow;
            RefreshButtonColors(canvasToShow);
        }

        private void SlideIn(Canvas canvas)
        {
            var (rect, originalPos) = GetRectAndPos(canvas);
            if (rect == null) return;
            rect.DOKill();
            rect.anchoredPosition = originalPos + new Vector2(_slideDistance, 0f);
            rect.DOAnchorPos(originalPos, _tweenDuration).SetUpdate(true).SetEase(Ease.OutCubic);
        }

        private void SlideOut(Canvas canvas)
        {
            var (rect, originalPos) = GetRectAndPos(canvas);
            if (rect == null) return;
            rect.DOKill();
            rect.DOAnchorPos(originalPos + new Vector2(-_slideDistance * 10f, 0f), _tweenDuration)
                .SetUpdate(true).SetEase(Ease.InCubic);
        }

        private void SlideOffScreen(RectTransform rect, Vector2 originalPos)
        {
            rect.anchoredPosition = originalPos + new Vector2(_slideDistance * 10f, 0f);
        }

        private (RectTransform rect, Vector2 pos) GetRectAndPos(Canvas canvas)
        {
            if (canvas == _skillTreeCanvas) return (_skillTreeRect, _skillTreeOriginalPos);
            if (canvas == _inventoryCanvas)  return (_inventoryRect, _inventoryOriginalPos);
            if (canvas == _questCanvas)      return (_questRect, _questOriginalPos);
            return (null, Vector2.zero);
        }

        private void RefreshButtonColors(Canvas active)
        {
            if (_btnToSkillTree != null)
                _btnToSkillTree.image.color = active == _skillTreeCanvas ? _selectedColor : _normalColor;
            if (_btnToInventory != null)
                _btnToInventory.image.color = active == _inventoryCanvas  ? _selectedColor : _normalColor;
            if (_btnToQuest != null)
                _btnToQuest.image.color     = active == _questCanvas      ? _selectedColor : _normalColor;
        }
    }
}
