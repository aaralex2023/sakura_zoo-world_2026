using System;
using UnityEngine;
using Zenject;
using ZooWorld.World;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateSwitchIfOutOfScreen : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        [SerializeField] private AnimalStateId _nextState;

        private IScreenBounds _screenBounds;
        private AnimalRuntimeState _state;
        private ScreenReturnConfig _config;
        private bool _switchEnabled;
        private bool _transitionRequested;

        [Inject]
        private void Construct(IScreenBounds screenBounds)
        {
            _screenBounds = screenBounds ?? throw new ArgumentNullException(nameof(screenBounds));
        }

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_screenBounds == null)
                throw new InvalidOperationException($"'{name}' has no injected screen bounds.");
            if (_nextState == null)
                throw new InvalidOperationException($"'{name}' has no next state ID.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no ScreenReturnConfig.");

            _state = context.State;
        }

        public void OnEnter()
        {
            _switchEnabled = _screenBounds.Contains(_state.Transform.position);
            _transitionRequested = false;
        }

        public void OnExit()
        {
            _switchEnabled = false;
            _transitionRequested = false;
        }

        public void OnTick(float dt)
        {
            if (_state.IsDead || !_switchEnabled || _transitionRequested)
                return;

            if (!_screenBounds.Contains(_state.Transform.position, _config.OutsideMargin))
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
