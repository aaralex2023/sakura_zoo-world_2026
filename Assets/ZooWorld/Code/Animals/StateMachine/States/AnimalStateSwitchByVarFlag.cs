using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateSwitchByVarFlag : MonoBehaviour, IAnimalContextUser,
        IAnimalStateEnterListener, IAnimalStatePredictor
    {
        [SerializeField] private AnimalVarId _flagId;
        [SerializeField] private AnimalStateId _nextState;

        private AnimalRuntimeState _state;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_flagId == null || _nextState == null)
                throw new InvalidOperationException($"'{name}' requires a variable flag and a next state.");

            _state = context.State;
        }

        public void OnEnter()
        {
            _state.Vars.Remove(_flagId);
        }

        public bool TryPopNextState(out AnimalStateId state)
        {
            state = null;
            if (_state == null || _state.IsDead || !_state.Vars.Remove(_flagId))
                return false;

            state = _nextState;
            return true;
        }
    }
}
