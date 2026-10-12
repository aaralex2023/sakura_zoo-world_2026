using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class FoodChainConfigComponent : AAnimalConfigComponent<FoodChainConfig>
    {
        [InfoBox("Provides the animal's food-chain role, including whether it can eat other animals.")]
        [SerializeField, ReadOnly] private bool _about;
    }
}
