using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class JumpLocomotionConfig : IAnimalConfig
    {
        [SerializeField] private float _distance;
        [SerializeField] private float _airTime;

        public float Distance => _distance;
        public float AirTime => _airTime;
    }
}
