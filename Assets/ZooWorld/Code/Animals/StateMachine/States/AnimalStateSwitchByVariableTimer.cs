using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateSwitchByVariableTimer : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        [SerializeField] private AnimalStateId _nextState;
        [SerializeField] private AnimalVarId _timerId;

        private AnimalRuntimeState _state;
        private double? _timeRemaining;
        private bool _transitionRequested;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_nextState == null)
                throw new InvalidOperationException($"'{name}' has no next state ID.");

            _state = context.State;
        }

        public void OnEnter()
        {
            _timeRemaining = null;
            _transitionRequested = false;
            if (_timerId == null || !_state.Vars.TryGet(_timerId, out double duration))
                return;
            if (double.IsNaN(duration) || double.IsInfinity(duration))
                throw new InvalidOperationException($"Animal '{_state.Transform.name}' has an invalid timer value.");

            _timeRemaining = duration;
        }

        public void OnExit()
        {
            _transitionRequested = false;
            _timeRemaining = null;
        }

        public void OnTick(float dt)
        {
            if (_state.IsDead || _transitionRequested || !_timeRemaining.HasValue)
                return;

            _timeRemaining -= dt;
            if (_timeRemaining <= 0d)
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
