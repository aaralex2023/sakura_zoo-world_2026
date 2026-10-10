using ZooWorld.Animals;

namespace ZooWorld.World
{
    public interface IAnimalDespawner
    {
        void Despawn(IAnimal animal);
    }
}
