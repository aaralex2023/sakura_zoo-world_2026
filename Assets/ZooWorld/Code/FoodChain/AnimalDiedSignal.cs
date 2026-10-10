using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    public sealed class AnimalDiedSignal
    {
        public readonly IAnimal Animal;
        public readonly FoodChainRoleId Role;
        public readonly IAnimal Eater;

        public AnimalDiedSignal(IAnimal animal, FoodChainRoleId role, IAnimal eater)
        {
            Animal = animal;
            Role = role;
            Eater = eater;
        }
    }
}
