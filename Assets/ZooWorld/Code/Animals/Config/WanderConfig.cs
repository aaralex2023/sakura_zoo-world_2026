using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class WanderConfig : IAnimalConfig
    {
        [SerializeField] private Vector2 _headingChangeInterval;
        public Vector2 HeadingChangeInterval => _headingChangeInterval;
    }
}
