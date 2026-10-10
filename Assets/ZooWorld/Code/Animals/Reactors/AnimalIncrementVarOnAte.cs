using System;
using UnityEngine;
using Zenject;
using ZooWorld.FoodChain;

namespace ZooWorld.Animals
{
    public sealed class AnimalIncrementVarOnAte : MonoBehaviour, IAnimalContextUser, IAnimalAteListener
    {
        private IAnimal _animal;
        private AnimalVarId _rankVar;

        [Inject]
        private void Construct(FoodChainSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            _rankVar = settings.RankVar != null
                ? settings.RankVar
                : throw new InvalidOperationException($"'{name}' has no rank variable configured.");
        }

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_animal != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_rankVar == null)
                throw new InvalidOperationException($"'{name}' has no injected rank variable.");

            _animal = context.Animal;
        }

        public void OnAte(IAnimal victim)
        {
            if (_animal == null)
                throw new InvalidOperationException($"'{name}' has not received animal context.");
            if (victim == null)
                throw new ArgumentNullException(nameof(victim));

            _animal.Vars.Add(_rankVar, 1d);
        }
    }
}
