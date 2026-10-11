using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalConfigProvider : MonoBehaviour, IAnimalConfigProvider
    {
        [SerializeField] private GameObject _configsRoot;

        private readonly Dictionary<Type, IAnimalConfig> _configs = new();
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(AnimalConfigProvider)} on '{name}' was initialized more than once.");
            if (_configsRoot == null)
                throw new InvalidOperationException($"{nameof(AnimalConfigProvider)} on '{name}' has no configs root reference.");

            AAnimalConfigComponent[] components = _configsRoot.GetComponents<AAnimalConfigComponent>();
            foreach (AAnimalConfigComponent component in components)
            {
                if (!component.isActiveAndEnabled)
                    continue;

                component.GetConfig(out Type type, out IAnimalConfig config);
                if (config == null)
                    throw new InvalidOperationException($"Config on '{component.name}' is null.");

                if (type == null || type != config.GetType())
                    throw new InvalidOperationException($"Config component '{component.name}' declared an invalid config type.");

                if (!_configs.TryAdd(type, config))
                    throw new InvalidOperationException($"Duplicate config of type '{type.Name}' on '{name}'.");
            }

            _isInitialized = true;
        }

        public bool TryGet<T>(out T config) where T : class, IAnimalConfig
        {
            if (!_isInitialized)
                throw new InvalidOperationException($"{nameof(AnimalConfigProvider)} on '{name}' is not initialized.");

            if (_configs.TryGetValue(typeof(T), out IAnimalConfig value))
            {
                config = (T)value;
                return true;
            }

            config = null;
            return false;
        }
    }
}
