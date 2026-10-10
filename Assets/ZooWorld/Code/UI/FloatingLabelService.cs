using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZooWorld.UI
{
    public sealed class FloatingLabelService : IFloatingLabelService
    {
        private readonly Canvas _canvas;
        private readonly Camera _camera;
        private readonly TMP_FontAsset _font;

        public FloatingLabelService(Canvas canvas, Camera camera, TMP_FontAsset font)
        {
            _canvas = canvas != null ? canvas : throw new ArgumentNullException(nameof(canvas));
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            _font = font != null ? font : throw new ArgumentNullException(nameof(font));
            if (_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                throw new ArgumentException("Floating labels require a Screen Space Overlay canvas.", nameof(canvas));
        }

        public void Show(string text, Transform anchor, Vector2 screenOffset, float duration)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Label text cannot be empty.", nameof(text));
            if (anchor == null)
                throw new ArgumentNullException(nameof(anchor));
            if (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "Label duration must be positive and finite.");

            GameObject labelObject = new(
                "Floating Label",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI),
                typeof(CanvasGroup),
                typeof(FloatingLabelView));
            labelObject.transform.SetParent(_canvas.transform, false);

            RectTransform rect = (RectTransform)labelObject.transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(240f, 52f);

            TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
            label.font = _font;
            label.fontSize = 32f;
            label.fontStyle = FontStyles.Bold;
            label.color = new Color(1f, 0.88f, 0.28f, 1f);
            label.alignment = TextAlignmentOptions.Center;
            label.text = text;
            label.raycastTarget = false;

            CanvasGroup canvasGroup = labelObject.GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            labelObject.GetComponent<FloatingLabelView>().Initialize(
                label, canvasGroup, _camera, (RectTransform)_canvas.transform, anchor, screenOffset, duration);
        }
    }
}
