using System;
using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    internal static class EncounterRoleUtility
    {
        public static FoodChainRoleId GetRole(IAnimal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            if (!animal.Configs.TryGet(out FoodChainConfig config) || config.Role == null)
                throw new InvalidOperationException($"Animal '{animal.Transform.name}' has no food-chain role.");

            return config.Role;
        }
    }
}
