using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class CrawlLocomotionConfig : IAnimalConfig
    {
        [SerializeField] private float _speed;

        public float Speed => _speed;
    }
}
