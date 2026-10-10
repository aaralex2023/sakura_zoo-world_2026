using System;
using UnityEngine;
using Zenject;
using ZooWorld.World;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateChooseHeading : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateTickListener
    {
        [SerializeField] private AnimalHeadingSelector _headingSelector;

        private AnimalRuntimeState _state;
        private WanderConfig _config;
        private IRandom _random;
        private float _timeUntilChange;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_headingSelector == null)
                throw new InvalidOperationException($"'{name}' has no heading selector reference.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no WanderConfig.");

            Vector2 interval = _config.HeadingChangeInterval;
            if (interval.x <= 0f || interval.y < interval.x ||
                float.IsNaN(interval.x) || float.IsInfinity(interval.x) ||
                float.IsNaN(interval.y) || float.IsInfinity(interval.y))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has an invalid heading change interval.");

            _state = context.State;
        }

        [Inject]
        private void Construct(IRandom random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public void OnEnter()
        {
            if (_random == null)
                throw new InvalidOperationException($"'{name}' has no injected random source.");

            _state.Heading = _headingSelector.ChooseHeading();
            ResetTimer();
        }

        public void OnTick(float dt)
        {
            if (_state.IsDead)
                return;

            _timeUntilChange -= dt;
            if (_headingSelector.HasObstaclePriority)
            {
                if (_timeUntilChange <= 0f)
                    ChooseAndReset();
                return;
            }

            if (_headingSelector.ShouldReturnToScreen())
            {
                ChooseAndReset();
                return;
            }

            if (_timeUntilChange <= 0f)
                ChooseAndReset();
        }

        private void ChooseAndReset()
        {
            _state.Heading = _headingSelector.ChooseHeading();
            ResetTimer();
        }

        private void ResetTimer()
        {
            _timeUntilChange = _random.Range(
                _config.HeadingChangeInterval.x,
                _config.HeadingChangeInterval.y);
        }
    }
}
