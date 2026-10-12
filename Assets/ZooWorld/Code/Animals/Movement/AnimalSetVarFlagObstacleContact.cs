using NaughtyAttributes;
using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalSetVarFlagObstacleContact : MonoBehaviour, IAnimalContextUser
    {
        [InfoBox("Creates the selected variable flag with value zero when an obstacle contact opposes the current heading.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private LayerMask _obstacleMask = 1 << 8;
        [SerializeField] private AnimalVarId _flagId;

        private AnimalRuntimeState _state;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_obstacleMask.value == 0 || _flagId == null)
                throw new InvalidOperationException($"'{name}' requires an obstacle mask and a variable flag.");

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
                if (TrySetFlag(collision.collider, collision.GetContact(i).normal))
                    return;
            }
        }

        private bool TrySetFlag(Collider obstacle, Vector3 normal)
        {
            if (_state.IsDead ||
                (_obstacleMask.value & (1 << obstacle.gameObject.layer)) == 0)
                return false;

            normal = Vector3.ProjectOnPlane(normal, Vector3.up);
            if (normal.sqrMagnitude <= 0.0001f || Vector3.Dot(_state.Heading, normal.normalized) >= -0.05f)
                return false;

            _state.Vars.Set(_flagId, 0d);
            return true;
        }
    }
}
