using System;
using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    public readonly struct Encounter
    {
        public readonly IAnimal A;
        public readonly IAnimal B;

        public Encounter(IAnimal a, IAnimal b)
        {
            A = a ?? throw new ArgumentNullException(nameof(a));
            B = b ?? throw new ArgumentNullException(nameof(b));
            if (ReferenceEquals(A, B))
                throw new ArgumentException("An encounter requires two different animals.", nameof(b));
        }
    }
}
