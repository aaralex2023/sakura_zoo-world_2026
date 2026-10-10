using UnityEngine;

namespace ZooWorld.World
{
    public interface IScreenBounds
    {
        Rect Area { get; }
        bool Contains(Vector3 position, float inset = 0f);
        Vector3 RandomPoint(float inset = 0f);
    }
}
