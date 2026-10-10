using UnityEngine;

namespace ZooWorld.Animals
{
    public interface IAnimal
    {
        Transform Transform { get; }
        Rigidbody Body { get; }
        bool IsDead { get; }
        IAnimalConfigProvider Configs { get; }
        IAnimalVars Vars { get; }

        void Initialize();
        bool TryDie(IAnimal eater);
        void NotifyAte(IAnimal victim);
        void NotifyEaten(IAnimal eater);
        bool IsInState(AnimalStateId state);
        void ForceSwitchState(AnimalStateId state);
    }
}
