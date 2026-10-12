using NaughtyAttributes;
using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateSwitchByVariableTimer : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        [InfoBox("Reads a duration from the selected variable on state entry and requests the next state when it expires.")]
        [SerializeField, ReadOnly] private bool _about;

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
            context.State.RequireState(_nextState);
            if (_timerId == null)
                throw new InvalidOperationException($"Timer transition on '{name}' has no timer variable ID. Remove the timer component for an indefinite wait.");
            GetDuration(context.State);

            _state = context.State;
        }

        public void OnEnter()
        {
            _transitionRequested = false;
            _timeRemaining = GetDuration(_state);
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

        private double GetDuration(AnimalRuntimeState state)
        {
            if (!state.Vars.TryGet(_timerId, out double duration))
                throw new InvalidOperationException($"Animal '{state.Transform.name}' has no value for timer variable '{_timerId.name}' used by state '{name}'.");
            if (duration < 0d || double.IsNaN(duration) || double.IsInfinity(duration))
                throw new InvalidOperationException($"Animal '{state.Transform.name}' requires a finite, non-negative value for timer variable '{_timerId.name}' used by state '{name}'.");

            return duration;
        }
    }
}
