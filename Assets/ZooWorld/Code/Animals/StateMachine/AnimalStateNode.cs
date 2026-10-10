using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalStateNode : MonoBehaviour
    {
        [SerializeField] private AnimalStateId _id;

        private IAnimalStateEnterListener[] _enterListeners = Array.Empty<IAnimalStateEnterListener>();
        private IAnimalStateExitListener[] _exitListeners = Array.Empty<IAnimalStateExitListener>();
        private IAnimalStateTickListener[] _tickListeners = Array.Empty<IAnimalStateTickListener>();
        private IAnimalStatePredictor[] _predictors = Array.Empty<IAnimalStatePredictor>();
        private AnimalRuntimeState _runtimeState;
        private bool _isPrepared;

        public AnimalStateId Id => _id;

        public void Prepare(AnimalContext context)
        {
            if (_isPrepared)
                throw new InvalidOperationException($"State '{name}' was prepared more than once.");
            if (_id == null)
                throw new InvalidOperationException($"State node '{name}' has no state ID.");
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            List<IAnimalContextUser> contextUsers = new();
            List<IAnimalStateEnterListener> enterListeners = new();
            List<IAnimalStateExitListener> exitListeners = new();
            List<IAnimalStateTickListener> tickListeners = new();
            List<IAnimalStatePredictor> predictors = new();
            _runtimeState = context.State;

            foreach (Component component in GetComponents<Component>())
            {
                if (component is IAnimalContextUser contextUser)
                    contextUsers.Add(contextUser);
                if (component is IAnimalStateEnterListener enterListener)
                    enterListeners.Add(enterListener);
                if (component is IAnimalStateExitListener exitListener)
                    exitListeners.Add(exitListener);
                if (component is IAnimalStateTickListener tickListener)
                    tickListeners.Add(tickListener);
                if (component is IAnimalStatePredictor predictor)
                    predictors.Add(predictor);
            }

            foreach (IAnimalContextUser contextUser in contextUsers)
                contextUser.SetupContext(context);

            _enterListeners = enterListeners.ToArray();
            _exitListeners = exitListeners.ToArray();
            _tickListeners = tickListeners.ToArray();
            _predictors = predictors.ToArray();
            _isPrepared = true;
        }

        public void Enter()
        {
            EnsurePrepared();
            foreach (IAnimalStateEnterListener listener in _enterListeners)
                listener.OnEnter();
        }

        public void Exit()
        {
            EnsurePrepared();
            foreach (IAnimalStateExitListener listener in _exitListeners)
                listener.OnExit();

            foreach (IAnimalStatePredictor predictor in _predictors)
                predictor.TryPopNextState(out _);
        }

        public void Tick(float dt)
        {
            EnsurePrepared();
            foreach (IAnimalStateTickListener listener in _tickListeners)
            {
                if (_runtimeState.IsDead)
                    return;
                listener.OnTick(dt);
            }
        }

        public bool TryPopNextState(out AnimalStateId state)
        {
            EnsurePrepared();
            state = null;
            bool found = false;

            foreach (IAnimalStatePredictor predictor in _predictors)
            {
                if (!predictor.TryPopNextState(out AnimalStateId requestedState))
                    continue;
                if (requestedState == null)
                    throw new InvalidOperationException($"A predictor on state '{name}' requested a null state ID.");

                if (!found)
                {
                    state = requestedState;
                    found = true;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"Multiple transition requests were raised by state '{name}'. The first component request wins.", this);
#endif
                }
            }

            return found;
        }

        private void EnsurePrepared()
        {
            if (!_isPrepared)
                throw new InvalidOperationException($"State node '{name}' has not been prepared.");
        }

        private void OnValidate()
        {
            if (_id != null && gameObject.name != _id.name)
                gameObject.name = _id.name;
        }
    }
}
