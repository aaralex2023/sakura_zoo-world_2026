namespace ZooWorld.Animals
{
    public interface IAnimalStatePredictor
    {
        bool TryPopNextState(out AnimalStateId state);
    }
}
