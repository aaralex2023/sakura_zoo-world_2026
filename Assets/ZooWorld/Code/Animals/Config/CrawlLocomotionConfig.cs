using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class CrawlLocomotionConfig : IAnimalConfig
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _duration;

        public float Speed => _speed;
        public float Acceleration => _acceleration;
        public float Duration => _duration;
    }
}
