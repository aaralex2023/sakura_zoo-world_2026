using UnityEngine;
using ZooWorld.Animals;

namespace ZooWorld.World
{
    public interface IAnimalFactory
    {
        IAnimal Create(AnimalDefinition definition, Vector3 position, Quaternion rotation);
    }
}
