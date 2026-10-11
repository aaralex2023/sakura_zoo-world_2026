using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalSwitchOnObstacleContact : MonoBehaviour, IAnimalContextUser
    {
        [SerializeField] private LayerMask _obstacleMask = 1 << 8;
        [SerializeField] private AnimalStateId _movementState;
        [SerializeField] private AnimalStateId _waitState;

        private AnimalRuntimeState _state;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_obstacleMask.value == 0 || _movementState == null || _waitState == null || _movementState == _waitState)
                throw new InvalidOperationException($"'{name}' requires an obstacle mask and distinct movement and wait states.");

            _state = context.State;
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleContact(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            HandleContact(collision);
        }

        private void HandleContact(Collision collision)
        {
            if (_state == null || _state.IsDead || collision == null || collision.collider == null)
                return;

            for (int i = 0; i < collision.contactCount; i++)
            {
                if (TryWait(collision.collider, collision.GetContact(i).normal))
                    return;
            }
        }

        private bool TryWait(Collider obstacle, Vector3 normal)
        {
            if (_state.IsDead || !_state.IsInState(_movementState) ||
                (_obstacleMask.value & (1 << obstacle.gameObject.layer)) == 0)
                return false;

            normal = Vector3.ProjectOnPlane(normal, Vector3.up);
            if (normal.sqrMagnitude <= 0.0001f || Vector3.Dot(_state.Heading, normal.normalized) >= -0.05f)
                return false;

            _state.ForceSwitchTo(_waitState);
            return true;
        }
    }
}
