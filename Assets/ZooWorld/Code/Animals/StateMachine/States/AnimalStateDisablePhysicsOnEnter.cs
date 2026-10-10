using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateDisablePhysicsOnEnter : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener
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

        public void OnEnter()
        {
            if (_state == null)
                throw new InvalidOperationException($"'{name}' has not received animal context.");

            _state.Body.linearVelocity = Vector3.zero;
            _state.Body.angularVelocity = Vector3.zero;
            _state.Collider.enabled = false;
            _state.Body.isKinematic = true;
        }
    }
}
