using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateWait : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        [SerializeField] private AnimalStateId _nextState;

        private AnimalRuntimeState _state;
        private WaitConfig _config;
        private float _timeRemaining;
        private bool _transitionRequested;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_nextState == null)
                throw new InvalidOperationException($"'{name}' has no next state ID.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no WaitConfig.");
            if (_config.Duration <= 0f || float.IsNaN(_config.Duration) || float.IsInfinity(_config.Duration))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has invalid wait settings.");

            _state = context.State;
        }

        public void OnEnter()
        {
            Vector3 velocity = _state.Body.linearVelocity;
            _state.Body.linearVelocity = new Vector3(0f, velocity.y, 0f);
            _state.Body.angularVelocity = Vector3.zero;
            _timeRemaining = _config.Duration;
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

            _timeRemaining -= dt;
            if (_timeRemaining <= 0f)
                _transitionRequested = true;
        }

        public bool TryPopNextState(out AnimalStateId state)
        {
            state = null;
            if (_state == null || _state.IsDead || !_transitionRequested)
                return false;

            _transitionRequested = false;
            state = _nextState;
            return true;
        }
    }
}
