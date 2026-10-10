using UnityEngine;

namespace ZooWorld.World
{
    public sealed class UnityRandom : IRandom
    {
        public float Range(float minInclusive, float maxInclusive)
        {
            return Random.Range(minInclusive, maxInclusive);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            return Random.Range(minInclusive, maxExclusive);
        }

        public bool Chance(float probability)
        {
            return Random.value < probability;
        }
    }
}
