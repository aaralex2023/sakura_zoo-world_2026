using NaughtyAttributes;
using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateSwitchByVarFlag : MonoBehaviour, IAnimalContextUser,
        IAnimalStateEnterListener, IAnimalStatePredictor
    {
        [InfoBox("Clears the selected flag on state entry, then consumes it and requests the next state when it appears again.\nThe flag's presence triggers the transition, regardless of its value.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private AnimalVarId _flagId;
        [SerializeField] private AnimalStateId _nextState;

        private AnimalRuntimeState _state;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            context.State.RequireState(_nextState);
            if (_flagId == null)
                throw new InvalidOperationException($"'{name}' requires a variable flag.");

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
