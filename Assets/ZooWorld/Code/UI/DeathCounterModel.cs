using System;
using System.Collections.Generic;
using Zenject;
using ZooWorld.FoodChain;

namespace ZooWorld.UI
{
    public sealed class DeathCounterModel : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly Dictionary<FoodChainRoleId, int> _counts = new();
        private bool _isInitialized;

        public event Action<FoodChainRoleId, int> Changed;

        public DeathCounterModel(SignalBus signalBus)
        {
            _signalBus = signalBus ?? throw new ArgumentNullException(nameof(signalBus));
        }

        public void Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(DeathCounterModel)} was initialized more than once.");

            _signalBus.Subscribe<AnimalDiedSignal>(OnAnimalDied);
            _isInitialized = true;
        }

        public int Get(FoodChainRoleId role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));
            return _counts.TryGetValue(role, out int count) ? count : 0;
        }

        public void Dispose()
        {
            if (!_isInitialized)
                return;

            _signalBus.Unsubscribe<AnimalDiedSignal>(OnAnimalDied);
            _isInitialized = false;
        }

        private void OnAnimalDied(AnimalDiedSignal signal)
        {
            if (signal == null)
                throw new ArgumentNullException(nameof(signal));
            if (signal.Role == null)
                throw new InvalidOperationException("Animal death signal has no role.");

            int count = Get(signal.Role) + 1;
            _counts[signal.Role] = count;
            Changed?.Invoke(signal.Role, count);
        }
    }
}
