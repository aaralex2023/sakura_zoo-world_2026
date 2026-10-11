using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateCrawlLocomotion : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        [SerializeField] private AnimalStateId _waitState;
        [SerializeField] private AnimalHeadingSelector _headingSelector;

        private AnimalRuntimeState _state;
        private CrawlLocomotionConfig _config;
        private bool _transitionRequested;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_waitState == null)
                throw new InvalidOperationException($"'{name}' has no Wait state ID.");
            if (_headingSelector == null)
                throw new InvalidOperationException($"'{name}' has no heading selector reference.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no CrawlLocomotionConfig.");
            if (_config.Speed <= 0f || _config.Acceleration <= 0f ||
                float.IsNaN(_config.Speed) || float.IsInfinity(_config.Speed) ||
                float.IsNaN(_config.Acceleration) || float.IsInfinity(_config.Acceleration))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has invalid crawl locomotion settings.");

            _state = context.State;
        }

        public void OnEnter()
        {
            _transitionRequested = false;
        }

        public void OnExit()
        {
            _transitionRequested = false;
        }

        public void OnTick(float dt)
        {
            if (_state.IsDead || _transitionRequested)
                return;

            Vector3 heading = Vector3.ProjectOnPlane(_state.Heading, Vector3.up);
            if (heading.sqrMagnitude <= 0.0001f)
                return;
            heading.Normalize();

            Vector3 velocity = _state.Body.linearVelocity;
            Vector3 currentVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            Vector3 desiredVelocity = heading * _config.Speed;
            Vector3 nextVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, _config.Acceleration * dt);
            _state.Body.linearVelocity = new Vector3(nextVelocity.x, velocity.y, nextVelocity.z);
            _state.Body.MoveRotation(Quaternion.LookRotation(heading, Vector3.up));

            if (_headingSelector.ShouldReturnToScreen() != _headingSelector.IsReturning)
                _transitionRequested = true;
        }

        public bool TryPopNextState(out AnimalStateId state)
        {
            state = null;
            if (_state == null || _state.IsDead || !_transitionRequested)
                return false;

            _transitionRequested = false;
            state = _waitState;
            return true;
        }
    }
}
