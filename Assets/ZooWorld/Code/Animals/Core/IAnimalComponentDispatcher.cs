namespace ZooWorld.Animals
{
    public interface IAnimalComponentDispatcher
    {
        void Eaten(IAnimal eater);
        void Ate(IAnimal victim);
    }
}
