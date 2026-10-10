namespace ZooWorld.Animals
{
    public interface IAnimalConfigProvider
    {
        bool TryGet<T>(out T config) where T : class, IAnimalConfig;
    }
}
