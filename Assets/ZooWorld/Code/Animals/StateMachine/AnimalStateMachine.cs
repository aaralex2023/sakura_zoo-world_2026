using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateMachine : MonoBehaviour
    {
        [SerializeField] private AnimalStateId _deadState;

        private readonly Dictionary<AnimalStateId, AnimalStateNode> _nodes = new();
        private AnimalRuntimeState _runtimeState;
        private AnimalStateNode _activeNode;
        private bool _isInitialized;
        private bool _isTransitioning;

        public AnimalStateId DeadState => _deadState;

        public void Initialize(AnimalContext context, AnimalStateId startState)
        {
            if (_isInitialized)
                throw new InvalidOperationException($"State machine '{name}' was initialized more than once.");
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_deadState == null)
                throw new InvalidOperationException($"State machine '{name}' has no Dead state ID.");
            if (startState == null)
                throw new InvalidOperationException($"Animal has no start state.");

            _runtimeState = context.State;
            AnimalStateNode[] nodes = GetComponentsInChildren<AnimalStateNode>(true);
            foreach (AnimalStateNode node in nodes)
            {
                if (node.Id == null)
                    throw new InvalidOperationException($"State node '{node.name}' has no state ID.");
                if (!_nodes.TryAdd(node.Id, node))
                    throw new InvalidOperationException($"State machine '{name}' has duplicate state ID '{node.Id.name}'.");
            }

            if (!_nodes.ContainsKey(_deadState))
                throw new InvalidOperationException($"State machine '{name}' has no node for Dead state '{_deadState.name}'.");
            if (!_nodes.ContainsKey(startState))
                throw new InvalidOperationException($"State machine '{name}' has no node for start state '{startState.name}'.");

            foreach (AnimalStateNode node in nodes)
                node.Prepare(context);

            _isInitialized = true;
            SwitchTo(startState);
        }

        public void LifeCycle(float dt)
        {
            EnsureInitialized();
            if (_runtimeState.IsDead)
                return;

            AnimalStateNode tickingNode = _activeNode;
            tickingNode.Tick(dt);
            if (_runtimeState.IsDead || _activeNode != tickingNode || !tickingNode.TryPopNextState(out AnimalStateId nextState))
                return;

            SwitchTo(nextState);
        }

        public bool IsInState(AnimalStateId state)
        {
            EnsureInitialized();
            return state != null && _activeNode.Id == state;
        }

        public void ForceSwitchState(AnimalStateId state)
        {
            EnsureInitialized();
            if (_runtimeState.IsDead && state != _deadState)
                return;
            SwitchTo(state);
        }

        private void SwitchTo(AnimalStateId state)
        {
            if (state == null || !_nodes.TryGetValue(state, out AnimalStateNode nextNode))
                throw new InvalidOperationException($"State machine '{name}' cannot switch to an unregistered state.");
            if (_activeNode == nextNode)
                return;
            if (_isTransitioning)
            {
                Debug.LogError($"Nested state transition on '{name}' was rejected.", this);
                return;
            }

            _isTransitioning = true;
            try
            {
                _activeNode?.Exit();
                _activeNode = nextNode;
                _activeNode.Enter();
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException($"State machine '{name}' has not been initialized.");
        }
    }
}
