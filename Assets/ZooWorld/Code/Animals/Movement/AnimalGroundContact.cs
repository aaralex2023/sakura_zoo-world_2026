using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalGroundContact : MonoBehaviour, IAnimalContextUser
    {
        private readonly HashSet<Collider> _groundColliders = new();
        private AnimalRuntimeState _state;
        private GroundContactConfig _config;

        public bool IsGrounded => _state != null && !_state.IsDead && _groundColliders.Count > 0;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no GroundContactConfig.");
            if (_config.GroundMask.value == 0 ||
                float.IsNaN(_config.MinGroundNormalY) || float.IsInfinity(_config.MinGroundNormalY) ||
                _config.MinGroundNormalY <= 0f || _config.MinGroundNormalY > 1f)
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has an invalid minimum ground normal.");

            _state = context.State;
        }

        private void OnCollisionEnter(Collision collision)
        {
            UpdateContact(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            UpdateContact(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_state == null || _state.IsDead || collision == null || collision.collider == null)
                return;

            _groundColliders.Remove(collision.collider);
        }

        private void UpdateContact(Collision collision)
        {
            if (_state == null || _state.IsDead || collision == null || collision.collider == null)
                return;
            if ((_config.GroundMask.value & (1 << collision.collider.gameObject.layer)) == 0)
            {
                _groundColliders.Remove(collision.collider);
                return;
            }

            bool hasUpwardContact = false;
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collision.GetContact(i).normal.y >= _config.MinGroundNormalY)
                {
                    hasUpwardContact = true;
                    break;
                }
            }

            if (hasUpwardContact)
                _groundColliders.Add(collision.collider);
            else
                _groundColliders.Remove(collision.collider);
        }
    }
}
