namespace ZooWorld.World
{
    public interface IRandom
    {
        float Range(float minInclusive, float maxInclusive);
        int Range(int minInclusive, int maxExclusive);
        bool Chance(float probability);
    }
}
