using System;
using UnityEngine;
using Zenject;
using ZooWorld.Animals;

namespace ZooWorld.World
{
    public sealed class AnimalFactory : IAnimalFactory
    {
        private readonly DiContainer _container;
        private readonly Transform _animalsRoot;

        public AnimalFactory(DiContainer container, Transform animalsRoot)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _animalsRoot = animalsRoot != null ? animalsRoot : throw new ArgumentNullException(nameof(animalsRoot));
        }

        public IAnimal Create(AnimalDefinition definition, Vector3 position, Quaternion rotation)
        {
            if (definition == null)
            {
                Debug.LogError("Cannot create an animal from a null definition.");
                return null;
            }
            if (definition.Prefab == null)
            {
                Debug.LogError($"Animal definition '{definition.name}' has no prefab.", definition);
                return null;
            }

            GameObject instance = null;
            try
            {
                instance = _container.InstantiatePrefab(definition.Prefab, position, rotation, _animalsRoot);
                IAnimal animal = instance.GetComponent<IAnimal>();
                if (animal == null)
                    throw new InvalidOperationException($"Prefab '{definition.Prefab.name}' has no root {nameof(IAnimal)} component.");

                animal.Initialize();
                return animal;
            }
            catch (Exception exception)
            {
                string instanceName = instance != null ? instance.name : definition.Prefab.name;
                Debug.LogError($"Failed to initialize animal '{instanceName}': {exception.Message}", instance);
                if (instance != null)
                {
                    instance.SetActive(false);
                    UnityEngine.Object.Destroy(instance);
                }

                return null;
            }
        }
    }
}
