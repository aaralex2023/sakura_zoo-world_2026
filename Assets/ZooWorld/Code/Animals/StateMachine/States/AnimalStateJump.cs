using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateJump : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        [SerializeField] private AnimalStateId _idleState;
        [SerializeField] private AnimalGroundContact _groundContact;

        private AnimalRuntimeState _state;
        private JumpLocomotionConfig _config;
        private bool _hasLeftGround;
        private bool _landed;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_idleState == null)
                throw new InvalidOperationException($"'{name}' has no Idle state ID.");
            if (_groundContact == null)
                throw new InvalidOperationException($"'{name}' has no ground contact reference.");
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
            _hasLeftGround = false;
            _landed = false;
        }

        public void OnExit()
        {
            _hasLeftGround = false;
            _landed = false;
        }

        public void OnTick(float dt)
        {
            if (_state.IsDead || _landed)
                return;

            if (!_hasLeftGround)
            {
                if (!_groundContact.IsGrounded)
                    _hasLeftGround = true;
                return;
            }

            if (!_groundContact.IsGrounded)
                return;

            _state.Body.linearVelocity = Vector3.zero;
            _state.Body.angularVelocity = Vector3.zero;
            _landed = true;
        }

        public bool TryPopNextState(out AnimalStateId state)
        {
            state = null;
            if (_state == null || _state.IsDead || !_landed)
                return false;

            _landed = false;
            state = _idleState;
            return true;
        }
    }
}
