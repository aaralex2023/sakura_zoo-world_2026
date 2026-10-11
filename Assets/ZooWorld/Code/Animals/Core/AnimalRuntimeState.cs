using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalRuntimeState
    {
        private readonly Func<AnimalStateId, bool> _isInState;
        private readonly Action<AnimalStateId> _forceSwitchTo;
        private readonly Action<AnimalStateId> _requireState;
        private bool _isDead;

        public Transform Transform { get; }
        public Rigidbody Body { get; }
        public Collider Collider { get; }
        public IAnimalVars Vars { get; }
        public Vector3 Heading { get; set; }
        public bool IsDead => _isDead;

        public AnimalRuntimeState(
            Transform transform,
            Rigidbody body,
            Collider collider,
            IAnimalVars vars,
            Func<AnimalStateId, bool> isInState,
            Action<AnimalStateId> forceSwitchTo,
            Action<AnimalStateId> requireState)
        {
            Transform = transform != null ? transform : throw new ArgumentNullException(nameof(transform));
            Body = body != null ? body : throw new ArgumentNullException(nameof(body));
            Collider = collider != null ? collider : throw new ArgumentNullException(nameof(collider));
            Vars = vars ?? throw new ArgumentNullException(nameof(vars));
            _isInState = isInState ?? throw new ArgumentNullException(nameof(isInState));
            _forceSwitchTo = forceSwitchTo ?? throw new ArgumentNullException(nameof(forceSwitchTo));
            _requireState = requireState ?? throw new ArgumentNullException(nameof(requireState));
        }

        public void RequireState(AnimalStateId id)
        {
            _requireState(id);
        }

        public void MarkDead()
        {
            _isDead = true;
        }

        public bool IsInState(AnimalStateId id)
        {
            return _isInState(id);
        }

        public void ForceSwitchTo(AnimalStateId id)
        {
            _forceSwitchTo(id);
        }
    }
}
