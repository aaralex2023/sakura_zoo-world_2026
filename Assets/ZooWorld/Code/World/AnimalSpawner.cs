using System;
using UnityEngine;
using Zenject;

namespace ZooWorld.World
{
    public sealed class AnimalSpawner : IInitializable, ITickable
    {
        private const int SpawnPositionAttempts = 5;

        private readonly AnimalCatalog _catalog;
        private readonly SpawnSettings _settings;
        private readonly IScreenBounds _screenBounds;
        private readonly IAnimalFactory _factory;
        private readonly IRandom _random;
        private float _timeUntilSpawn;
        private bool _isInitialized;

        public AnimalSpawner(
            AnimalCatalog catalog,
            SpawnSettings settings,
            IScreenBounds screenBounds,
            IAnimalFactory factory,
            IRandom random)
        {
            _catalog = catalog != null ? catalog : throw new ArgumentNullException(nameof(catalog));
            _settings = settings != null ? settings : throw new ArgumentNullException(nameof(settings));
            _screenBounds = screenBounds ?? throw new ArgumentNullException(nameof(screenBounds));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public void Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(AnimalSpawner)} was initialized more than once.");

            Vector2 intervalRange = _settings.IntervalRange;
            if (intervalRange.x <= 0f || intervalRange.y < intervalRange.x ||
                float.IsNaN(intervalRange.x) || float.IsNaN(intervalRange.y) ||
                float.IsInfinity(intervalRange.x) || float.IsInfinity(intervalRange.y))
                throw new InvalidOperationException("Spawn interval range must contain positive, finite values in ascending order.");

            if (float.IsNaN(_settings.SpawnInset) || float.IsInfinity(_settings.SpawnInset) ||
                float.IsNaN(_settings.SpawnHeight) || float.IsInfinity(_settings.SpawnHeight))
                throw new InvalidOperationException("Spawn inset and height must be finite values.");

            Rect area = _screenBounds.Area;
            if (area.width <= 2f * _settings.SpawnInset || area.height <= 2f * _settings.SpawnInset)
                throw new InvalidOperationException("Spawn inset leaves no screen area for animal placement.");

            _catalog.Validate();
            _timeUntilSpawn = _random.Range(intervalRange.x, intervalRange.y);
            _isInitialized = true;
        }

        public void Tick()
        {
            if (!_isInitialized)
                return;

            _timeUntilSpawn -= Time.deltaTime;
            if (_timeUntilSpawn > 0f)
                return;

            TrySpawn();
            _timeUntilSpawn = _random.Range(_settings.IntervalRange.x, _settings.IntervalRange.y);
        }

        private void TrySpawn()
        {
            AnimalDefinition definition = _catalog.PickWeighted(_random);
            for (int attempt = 0; attempt < SpawnPositionAttempts; attempt++)
            {
                Vector3 position = _screenBounds.RandomPoint(_settings.SpawnInset);
                position.y = _settings.SpawnHeight;
                if (Physics.CheckSphere(
                        position,
                        definition.SpawnRadius,
                        _settings.BlockingMask,
                        QueryTriggerInteraction.Ignore))
                    continue;

                Quaternion rotation = Quaternion.Euler(0f, _random.Range(0f, 360f), 0f);
                _factory.Create(definition, position, rotation);
                return;
            }
        }
    }
}
