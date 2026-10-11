using System;
using UnityEngine;
using Zenject;
using ZooWorld.FoodChain;

namespace ZooWorld.Animals
{
    public sealed class Animal : MonoBehaviour, IAnimal
    {
        [SerializeField] private AnimalStateMachine _stateMachine;
        [SerializeField] private AnimalComponentDispatcher _dispatcher;
        [SerializeField] private AnimalConfigProvider _configs;
        [SerializeField] private Rigidbody _body;
        [SerializeField] private AnimalStateId _startState;

        private SignalBus _signalBus;
        private AnimalRuntimeState _runtimeState;
        private AnimalVars _vars;
        private bool _isInitializing;
        private bool _isInitialized;

        public Transform Transform => transform;
        public Rigidbody Body => _body;
        public bool IsDead => _runtimeState != null && _runtimeState.IsDead;
        public IAnimalConfigProvider Configs => _configs;
        public IAnimalVars Vars => _vars;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            if (_isInitialized || _isInitializing)
                throw new InvalidOperationException($"Animal '{name}' was initialized more than once.");

            _isInitializing = true;
            try
            {
                ValidateRequiredReferences();
                if (_signalBus == null)
                    throw new InvalidOperationException($"Animal '{name}' has no injected SignalBus.");

                _configs.Initialize();
                if (!_configs.TryGet(out FoodChainConfig foodChainConfig) || foodChainConfig.Role == null)
                    throw new InvalidOperationException($"Animal '{name}' requires a FoodChainConfig with a role.");

                _vars = new AnimalVars();
                if (_configs.TryGet(out AnimalVarsInitConfig varsConfig))
                {
                    foreach (VarInit value in varsConfig.Values)
                    {
                        if (value.Id == null)
                            throw new InvalidOperationException($"Animal '{name}' has a null variable ID in its initial values.");
                        _vars.Set(value.Id, value.Value);
                    }
                }

                Collider collider = GetComponent<Collider>();
                if (collider == null)
                    throw new InvalidOperationException($"Animal '{name}' is missing a root Collider.");

                _runtimeState = new AnimalRuntimeState(
                    transform,
                    _body,
                    collider,
                    _vars,
                    IsInState,
                    ForceSwitchState,
                    _stateMachine.RequireState);
                AnimalContext context = new(this, _runtimeState, _configs);

                _dispatcher.Init(context);
                foreach (IAnimalContextUser contextUser in ComponentExtractorUtil.ExtractDirectChildren<IAnimalContextUser>(gameObject, _dispatcher.ReactorsRoot))
                    contextUser.SetupContext(context);

                _stateMachine.Initialize(context, _startState);
                _isInitialized = true;
            }
            finally
            {
                _isInitializing = false;
            }
        }

        public bool TryDie(IAnimal eater)
        {
            EnsureInitialized();
            if (IsDead)
                return false;

            _runtimeState.MarkDead();
            _stateMachine.ForceSwitchState(_stateMachine.DeadState);
            _configs.TryGet(out FoodChainConfig foodChainConfig);
            _signalBus.Fire(new AnimalDiedSignal(this, foodChainConfig.Role, eater));
            return true;
        }

        public void NotifyAte(IAnimal victim)
        {
            EnsureInitialized();
            _dispatcher.Ate(victim);
        }

        public void NotifyEaten(IAnimal eater)
        {
            EnsureInitialized();
            _dispatcher.Eaten(eater);
        }

        public bool IsInState(AnimalStateId state)
        {
            return _stateMachine != null && _stateMachine.IsInState(state);
        }

        public void ForceSwitchState(AnimalStateId state)
        {
            EnsureInitialized();
            _stateMachine.ForceSwitchState(state);
        }

        private void FixedUpdate()
        {
            if (_isInitialized && !IsDead)
                _stateMachine.LifeCycle(Time.fixedDeltaTime);
        }

        private void ValidateRequiredReferences()
        {
            if (_stateMachine == null)
                throw new InvalidOperationException($"Animal '{name}' is missing its state machine reference.");
            if (_dispatcher == null)
                throw new InvalidOperationException($"Animal '{name}' is missing its component dispatcher reference.");
            if (_configs == null)
                throw new InvalidOperationException($"Animal '{name}' is missing its config provider reference.");
            if (_body == null)
                throw new InvalidOperationException($"Animal '{name}' is missing its Rigidbody reference.");
            if (_startState == null)
                throw new InvalidOperationException($"Animal '{name}' is missing its start state ID.");
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException($"Animal '{name}' has not been initialized.");
        }
    }
}
