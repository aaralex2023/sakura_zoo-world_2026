using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateSwitchIfGrounded : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        [SerializeField] private AnimalStateId _nextState;
        [SerializeField] private AnimalGroundContact _groundContact;
        [SerializeField] private float _minimumJumpDuration = 0.2f;

        private AnimalRuntimeState _state;
        private float _timeInState;
        private bool _transitionRequested;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_nextState == null)
                throw new InvalidOperationException($"'{name}' has no next state ID.");
            if (_groundContact == null)
                throw new InvalidOperationException($"'{name}' has no ground contact reference.");
            if (_minimumJumpDuration < 0f || float.IsNaN(_minimumJumpDuration) || float.IsInfinity(_minimumJumpDuration))
                throw new InvalidOperationException($"'{name}' has an invalid minimum jump duration.");

            _state = context.State;
        }

        public void OnEnter()
        {
            _timeInState = 0f;
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

            _timeInState += dt;
            if (_timeInState >= _minimumJumpDuration && _groundContact.IsGrounded)
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
