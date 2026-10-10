using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateCrawlLocomotion : MonoBehaviour, IAnimalContextUser, IAnimalStateTickListener
    {
        private AnimalRuntimeState _state;
        private CrawlLocomotionConfig _config;

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_state != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no CrawlLocomotionConfig.");
            if (_config.Speed <= 0f || _config.Acceleration <= 0f ||
                float.IsNaN(_config.Speed) || float.IsInfinity(_config.Speed) ||
                float.IsNaN(_config.Acceleration) || float.IsInfinity(_config.Acceleration))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has invalid crawl locomotion settings.");

            _state = context.State;
        }

        public void OnTick(float dt)
        {
            if (_state.IsDead)
                return;

            Vector3 heading = Vector3.ProjectOnPlane(_state.Heading, Vector3.up);
            if (heading.sqrMagnitude <= 0.0001f)
                return;
            heading.Normalize();

            Vector3 currentVelocity = Vector3.ProjectOnPlane(_state.Body.linearVelocity, Vector3.up);
            Vector3 desiredVelocity = heading * _config.Speed;
            Vector3 nextVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, _config.Acceleration * dt);
            _state.Body.AddForce(nextVelocity - currentVelocity, ForceMode.VelocityChange);
            _state.Body.MoveRotation(Quaternion.LookRotation(heading, Vector3.up));
        }
    }
}
