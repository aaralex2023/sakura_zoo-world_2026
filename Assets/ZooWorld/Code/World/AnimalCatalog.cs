using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZooWorld.World
{
    [CreateAssetMenu(menuName = "ZooWorld/Animal Catalog")]
    public sealed class AnimalCatalog : ScriptableObject
    {
        [InfoBox("Lists the animal definitions available for spawning.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private List<AnimalDefinition> _animals = new();

        public AnimalDefinition PickWeighted(IRandom random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            float totalWeight = ValidateAndGetTotalWeight();
            float selection = random.Range(0f, totalWeight);
            float cumulative = 0f;
            AnimalDefinition lastWeighted = null;

            foreach (AnimalDefinition animal in _animals)
            {
                if (animal.SpawnWeight <= 0f)
                    continue;

                lastWeighted = animal;
                cumulative += animal.SpawnWeight;
                if (selection < cumulative)
                    return animal;
            }

            return lastWeighted;
        }

        public void Validate()
        {
            ValidateAndGetTotalWeight();
        }

        private float ValidateAndGetTotalWeight()
        {
            if (_animals == null || _animals.Count == 0)
                throw new InvalidOperationException($"Animal catalog '{name}' has no definitions.");

            float totalWeight = 0f;
            foreach (AnimalDefinition animal in _animals)
            {
                if (animal == null)
                    throw new InvalidOperationException($"Animal catalog '{name}' contains a missing definition.");
                if (animal.Prefab == null)
                    throw new InvalidOperationException($"Animal definition '{animal.name}' has no prefab.");
                if (animal.SpawnWeight < 0f || float.IsNaN(animal.SpawnWeight) || float.IsInfinity(animal.SpawnWeight))
                    throw new InvalidOperationException($"Animal definition '{animal.name}' has an invalid spawn weight.");
                if (animal.SpawnRadius <= 0f || float.IsNaN(animal.SpawnRadius) || float.IsInfinity(animal.SpawnRadius))
                    throw new InvalidOperationException($"Animal definition '{animal.name}' has an invalid spawn radius.");

                totalWeight += animal.SpawnWeight;
            }

            if (totalWeight <= 0f || float.IsInfinity(totalWeight))
                throw new InvalidOperationException($"Animal catalog '{name}' has no positive spawn weight.");

            return totalWeight;
        }
    }
}
