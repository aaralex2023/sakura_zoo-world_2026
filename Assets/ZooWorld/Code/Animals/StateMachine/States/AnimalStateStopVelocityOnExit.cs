using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateStopVelocityOnExit : MonoBehaviour, IAnimalContextUser, IAnimalStateExitListener
    {
        private AnimalRuntimeState _state;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");

            _state = context.State;
        }

        public void OnExit()
        {
            _state.Body.linearVelocity = Vector3.zero;
            _state.Body.angularVelocity = Vector3.zero;
        }
    }
}
