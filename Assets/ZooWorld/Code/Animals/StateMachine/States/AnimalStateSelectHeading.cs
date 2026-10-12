using NaughtyAttributes;
using System;
using UnityEngine;
using Zenject;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateSelectHeading : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStatePredictor
    {
        [InfoBox("Chooses a random heading inside the inner screen area, or heads toward the center near the edges.\nThen requests the configured next state.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private AnimalStateId _nextState;
        private AnimalHeadingSelector _headingSelector;
        private AnimalRuntimeState _state;
        private bool _transitionRequested;

        [Inject]
        private void Construct(AnimalHeadingSelector headingSelector)
        {
            _headingSelector = headingSelector ?? throw new ArgumentNullException(nameof(headingSelector));
        }

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            context.State.RequireState(_nextState);
            if (_headingSelector == null)
                throw new InvalidOperationException($"'{name}' has no injected heading selector.");

            _state = context.State;
        }

        public void OnEnter()
        {
            if (_state.IsDead)
                return;

            _state.Heading = _headingSelector.ChooseHeading(_state.Transform.position);
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
