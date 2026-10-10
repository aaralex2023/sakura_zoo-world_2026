using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateIdle : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        [SerializeField] private AnimalStateId _jumpState;
        [SerializeField] private AnimalHeadingSelector _headingSelector;
        [SerializeField] private AnimalGroundContact _groundContact;

        private AnimalRuntimeState _state;
        private JumpLocomotionConfig _config;
        private float _timeRemaining;
        private bool _transitionRequested;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_jumpState == null)
                throw new InvalidOperationException($"'{name}' has no Jump state ID.");
            if (_headingSelector == null || _groundContact == null)
                throw new InvalidOperationException($"'{name}' is missing its heading selector or ground contact reference.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no JumpLocomotionConfig.");
            ValidateConfig(context.Animal.Transform.name, _config);

            _state = context.State;
        }

        public void OnEnter()
        {
            _timeRemaining = _config.IdleDuration;
            _transitionRequested = false;
        }

        public void OnExit()
        {
            _transitionRequested = false;
        }

        public void OnTick(float dt)
        {
            if (_state.IsDead || _transitionRequested || !_groundContact.IsGrounded)
                return;

            _timeRemaining -= dt;
            if (_timeRemaining > 0f)
                return;

            _state.Heading = _headingSelector.ChooseHeadingForJump();
            _transitionRequested = true;
        }

        public bool TryPopNextState(out AnimalStateId state)
        {
            state = null;
            if (_state == null || _state.IsDead || !_transitionRequested)
                return false;

            _transitionRequested = false;
            state = _jumpState;
            return true;
        }

        private static void ValidateConfig(string animalName, JumpLocomotionConfig config)
        {
            if (config.IdleDuration <= 0f || config.Distance <= 0f || config.AirTime <= 0f ||
                float.IsNaN(config.IdleDuration) || float.IsInfinity(config.IdleDuration) ||
                float.IsNaN(config.Distance) || float.IsInfinity(config.Distance) ||
                float.IsNaN(config.AirTime) || float.IsInfinity(config.AirTime))
                throw new InvalidOperationException($"Animal '{animalName}' has invalid jump locomotion settings.");
        }
    }
}
