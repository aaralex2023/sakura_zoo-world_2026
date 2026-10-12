using System;
using UnityEngine;
using ZooWorld.World;

namespace ZooWorld.Animals
{
    public sealed class AnimalHeadingSelector
    {
        private const float DirectionEpsilon = 0.0001f;

        private IScreenBounds _screenBounds;
        private IRandom _random;
        private ScreenReturnConfig _screenReturnConfig;

        public AnimalHeadingSelector(IScreenBounds screenBounds, IRandom random, ScreenReturnConfig screenReturnConfig)
        {
            _screenBounds = screenBounds ?? throw new ArgumentNullException(nameof(screenBounds));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _screenReturnConfig = screenReturnConfig ?? throw new ArgumentNullException(nameof(screenReturnConfig));
            if (float.IsNaN(_screenReturnConfig.OutsideMargin) || float.IsInfinity(_screenReturnConfig.OutsideMargin) ||
                float.IsNaN(_screenReturnConfig.InsideMargin) || float.IsInfinity(_screenReturnConfig.InsideMargin) ||
                _screenReturnConfig.InsideMargin < _screenReturnConfig.OutsideMargin)
                throw new InvalidOperationException("Screen return settings have invalid margins.");
            Rect area = _screenBounds.Area;
            if (area.width <= 2f * _screenReturnConfig.InsideMargin ||
                area.height <= 2f * _screenReturnConfig.InsideMargin)
                throw new InvalidOperationException("Screen return margins leave no interior area.");
        }

        public Vector3 ChooseHeading(Vector3 from)
        {
            from.y = 0f;
            if (!_screenBounds.Contains(from, _screenReturnConfig.InsideMargin))
            {
                Rect area = _screenBounds.Area;
                Vector3 screenCenter = new Vector3(area.center.x, 0f, area.center.y);
                return DirectionTo(screenCenter - from);
            }

            float angle = _random.Range(0f, 360f) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        }

        private static Vector3 DirectionTo(Vector3 direction)
        {
            direction.y = 0f;
            return direction.sqrMagnitude > DirectionEpsilon ? direction.normalized : Vector3.forward;
        }
    }
}
