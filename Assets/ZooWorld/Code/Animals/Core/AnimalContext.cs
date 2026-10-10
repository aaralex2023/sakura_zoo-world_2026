using System;

namespace ZooWorld.Animals
{
    public sealed class AnimalContext
    {
        public readonly IAnimal Animal;
        public readonly AnimalRuntimeState State;
        public readonly IAnimalConfigProvider Configs;

        public AnimalContext(IAnimal animal, AnimalRuntimeState state, IAnimalConfigProvider configs)
        {
            Animal = animal ?? throw new ArgumentNullException(nameof(animal));
            State = state ?? throw new ArgumentNullException(nameof(state));
            Configs = configs ?? throw new ArgumentNullException(nameof(configs));
        }
    }
}
