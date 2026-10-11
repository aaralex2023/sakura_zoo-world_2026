using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    public interface IEncounterResolver
    {
        bool TryResolve(IAnimal a, IAnimal b, out IAnimal eater, out IAnimal victim);
    }
}
