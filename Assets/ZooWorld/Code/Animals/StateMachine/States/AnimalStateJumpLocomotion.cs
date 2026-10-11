using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateJumpLocomotion : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener
    {
        private AnimalRuntimeState _state;
        private JumpLocomotionConfig _config;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no JumpLocomotionConfig.");
            if (_config.Distance <= 0f || _config.AirTime <= 0f ||
                float.IsNaN(_config.Distance) || float.IsInfinity(_config.Distance) ||
                float.IsNaN(_config.AirTime) || float.IsInfinity(_config.AirTime))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has invalid jump locomotion settings.");

            _state = context.State;
        }

        public void OnEnter()
        {
            float gravity = -Physics.gravity.y;
            if (gravity <= 0f || float.IsNaN(gravity) || float.IsInfinity(gravity) ||
                float.IsNaN(Physics.gravity.x) || float.IsInfinity(Physics.gravity.x) ||
                float.IsNaN(Physics.gravity.z) || float.IsInfinity(Physics.gravity.z) ||
                Mathf.Abs(Physics.gravity.x) > 0.0001f || Mathf.Abs(Physics.gravity.z) > 0.0001f)
                throw new InvalidOperationException($"Animal '{_state.Transform.name}' requires downward world gravity.");

            Vector3 heading = Vector3.ProjectOnPlane(_state.Heading, Vector3.up);
            if (heading.sqrMagnitude <= 0.0001f)
                throw new InvalidOperationException($"Animal '{_state.Transform.name}' has no jump heading.");
            heading.Normalize();

            float horizontalSpeed = _config.Distance / _config.AirTime;
            float verticalSpeed = gravity * _config.AirTime * 0.5f;
            _state.Body.MoveRotation(Quaternion.LookRotation(heading, Vector3.up));
            _state.Body.linearVelocity = heading * horizontalSpeed + Vector3.up * verticalSpeed;
        }
    }
}
