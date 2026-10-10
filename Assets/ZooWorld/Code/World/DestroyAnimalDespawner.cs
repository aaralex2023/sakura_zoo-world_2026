using UnityEngine;
using ZooWorld.Animals;

namespace ZooWorld.World
{
    public sealed class DestroyAnimalDespawner : IAnimalDespawner
    {
        public void Despawn(IAnimal animal)
        {
            if (animal == null || animal.Transform == null)
                return;

            UnityEngine.Object.Destroy(animal.Transform.gameObject);
        }
    }
}
