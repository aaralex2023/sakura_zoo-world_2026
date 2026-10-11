using System;
using UnityEngine;
using Zenject;
using ZooWorld.World;

namespace ZooWorld.Animals
{
    public sealed class AnimalHeadingSelector : MonoBehaviour, IAnimalContextUser
    {
        private const float DirectionEpsilon = 0.0001f;

        private IScreenBounds _screenBounds;
        private IRandom _random;
        private AnimalRuntimeState _state;
        private ScreenReturnConfig _screenReturnConfig;

        [Inject]
        private void Construct(IScreenBounds screenBounds, IRandom random)
        {
            _screenBounds = screenBounds ?? throw new ArgumentNullException(nameof(screenBounds));
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_screenBounds == null || _random == null)
                throw new InvalidOperationException($"'{name}' has no injected screen bounds or random source.");
            if (!context.Configs.TryGet(out _screenReturnConfig))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no ScreenReturnConfig.");
            if (float.IsNaN(_screenReturnConfig.OutsideMargin) || float.IsInfinity(_screenReturnConfig.OutsideMargin) ||
                float.IsNaN(_screenReturnConfig.InsideMargin) || float.IsInfinity(_screenReturnConfig.InsideMargin) ||
                _screenReturnConfig.InsideMargin < _screenReturnConfig.OutsideMargin)
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has invalid screen return margins.");
            Rect area = _screenBounds.Area;
            if (area.width <= 2f * _screenReturnConfig.InsideMargin ||
                area.height <= 2f * _screenReturnConfig.InsideMargin)
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has screen return margins that leave no interior area.");

            _state = context.State;
        }

        public Vector3 ChooseHeading()
        {
            EnsureReady();
            Vector3 position = _state.Transform.position;
            position.y = 0f;
            if (!_screenBounds.Contains(position, _screenReturnConfig.InsideMargin))
            {
                Rect area = _screenBounds.Area;
                Vector3 screenCenter = new Vector3(area.center.x, 0f, area.center.y);
                return DirectionTo(screenCenter - position);
            }

            float angle = _random.Range(0f, 360f) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        }

        private static Vector3 DirectionTo(Vector3 direction)
        {
            direction.y = 0f;
            return direction.sqrMagnitude > DirectionEpsilon ? direction.normalized : Vector3.forward;
        }

        private void EnsureReady()
        {
            if (_state == null || _screenBounds == null || _random == null || _screenReturnConfig == null)
                throw new InvalidOperationException($"'{name}' has not been initialized.");
        }
    }
}
