using System;
using UnityEngine;
using Zenject;
using ZooWorld.World;

namespace ZooWorld.Animals
{
    public sealed class AnimalHeadingSelector : MonoBehaviour, IAnimalContextUser
    {
        private const float DirectionEpsilon = 0.0001f;
        private const float ObstacleDeflectionAngle = 25f;

        [SerializeField] private LayerMask _obstacleMask = 1 << 8;

        private IScreenBounds _screenBounds;
        private IRandom _random;
        private AnimalRuntimeState _state;
        private ScreenReturnConfig _screenReturnConfig;
        private bool _hasObstaclePriority;
        private Collider _lastObstacleCollider;
        private Vector3 _lastObstacleNormal;

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
            if (_obstacleMask.value == 0)
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has an empty obstacle collision mask.");
            Rect area = _screenBounds.Area;
            if (area.width <= 2f * _screenReturnConfig.InsideMargin ||
                area.height <= 2f * _screenReturnConfig.InsideMargin)
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has screen return margins that leave no interior area.");

            _state = context.State;
        }

        public Vector3 ChooseHeading()
        {
            EnsureReady();
            if (_hasObstaclePriority)
            {
                _hasObstaclePriority = false;
                return _state.Heading;
            }

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

        private void OnCollisionEnter(Collision collision)
        {
            HandleObstacleContact(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            HandleObstacleContact(collision);
        }

        private void HandleObstacleContact(Collision collision)
        {
            if (_state == null || _state.IsDead || collision == null || collision.collider == null)
                return;
            Collider obstacleCollider = collision.collider;
            if ((_obstacleMask.value & (1 << obstacleCollider.gameObject.layer)) == 0)
                return;

            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint contact = collision.GetContact(i);
                Vector3 normal = Vector3.ProjectOnPlane(contact.normal, Vector3.up);
                if (normal.sqrMagnitude <= DirectionEpsilon)
                    continue;
                normal.Normalize();

                Vector3 awayFromContact = Vector3.ProjectOnPlane(transform.position - contact.point, Vector3.up);
                if (awayFromContact.sqrMagnitude > DirectionEpsilon && Vector3.Dot(normal, awayFromContact) < 0f)
                    normal = -normal;

                bool newSurface = obstacleCollider != _lastObstacleCollider ||
                                  Vector3.Dot(normal, _lastObstacleNormal) < 0.9f;
                bool headingIntoSurface = Vector3.Dot(_state.Heading, normal) < -0.05f;
                if (!newSurface && !headingIntoSurface)
                    continue;

                float angle = _random.Range(-ObstacleDeflectionAngle, ObstacleDeflectionAngle);
                Vector3 heading = Quaternion.AngleAxis(angle, Vector3.up) * normal;
                _state.Heading = DirectionTo(heading);
                _lastObstacleCollider = obstacleCollider;
                _lastObstacleNormal = normal;
                _hasObstaclePriority = true;
                return;
            }
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
