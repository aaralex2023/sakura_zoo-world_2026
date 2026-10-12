using NaughtyAttributes;
using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateWait : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener
    {
        [InfoBox("Stops horizontal movement and rotation on state entry, preserving vertical velocity.")]
        [SerializeField, ReadOnly] private bool _about;

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
            Vector3 velocity = _state.Body.linearVelocity;
            _state.Body.linearVelocity = new Vector3(0f, velocity.y, 0f);
            _state.Body.angularVelocity = Vector3.zero;
        }
    }
}
