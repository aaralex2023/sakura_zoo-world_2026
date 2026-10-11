using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateSelectHeading : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStatePredictor
    {
        [SerializeField] private AnimalStateId _nextState;
        [SerializeField] private AnimalHeadingSelector _headingSelector;

        private AnimalRuntimeState _state;
        private bool _transitionRequested;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            context.State.RequireState(_nextState);
            if (_headingSelector == null)
                throw new InvalidOperationException($"'{name}' has no heading selector reference.");

            _state = context.State;
        }

        public void OnEnter()
        {
            if (_state.IsDead)
                return;

            _state.Heading = _headingSelector.ChooseHeading();
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
