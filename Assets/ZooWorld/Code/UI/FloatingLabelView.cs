using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;

namespace ZooWorld.UI
{
    public sealed class FloatingLabelView : MonoBehaviour
    {
        [InfoBox("Follows a world anchor on screen, fades over its lifetime, and destroys itself when finished.")]
        [SerializeField, ReadOnly] private bool _about;

        private TMP_Text _label;
        private CanvasGroup _canvasGroup;
        private Camera _camera;
        private RectTransform _canvasRect;
        private Transform _anchor;
        private Vector2 _screenOffset;
        private Vector2 _lastScreenPosition;
        private float _duration;
        private float _elapsed;
        private bool _hasScreenPosition;

        public void Initialize(
            TMP_Text label,
            CanvasGroup canvasGroup,
            Camera camera,
            RectTransform canvasRect,
            Transform anchor,
            Vector2 screenOffset,
            float duration)
        {
            if (_label != null)
                throw new InvalidOperationException($"'{name}' was initialized more than once.");
            _label = label != null ? label : throw new ArgumentNullException(nameof(label));
            _canvasGroup = canvasGroup != null ? canvasGroup : throw new ArgumentNullException(nameof(canvasGroup));
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            _canvasRect = canvasRect != null ? canvasRect : throw new ArgumentNullException(nameof(canvasRect));
            _anchor = anchor != null ? anchor : throw new ArgumentNullException(nameof(anchor));
            if (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "Label duration must be positive and finite.");

            _screenOffset = screenOffset;
            _duration = duration;
        }

        private void LateUpdate()
        {
            if (_anchor != null)
            {
                Vector3 screenPosition = _camera.WorldToScreenPoint(_anchor.position);
                _label.enabled = screenPosition.z > 0f;
                if (_label.enabled)
                {
                    _lastScreenPosition = new Vector2(screenPosition.x, screenPosition.y) + _screenOffset;
                    _hasScreenPosition = true;
                }
            }
            else
            {
                _label.enabled = _hasScreenPosition;
            }

            if (_hasScreenPosition && RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect, _lastScreenPosition, null, out Vector2 localPosition))
                ((RectTransform)transform).anchoredPosition = localPosition;

            _elapsed += Time.deltaTime;
            _canvasGroup.alpha = 1f - Mathf.Clamp01(_elapsed / _duration);
            if (_elapsed >= _duration)
                Destroy(gameObject);
        }
    }
}
