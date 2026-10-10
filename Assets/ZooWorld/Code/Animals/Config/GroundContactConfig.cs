using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class GroundContactConfig : IAnimalConfig
    {
        [SerializeField] private LayerMask _groundMask = 1 << 7;
        [SerializeField] private float _minGroundNormalY = 0.6f;

        public LayerMask GroundMask => _groundMask;
        public float MinGroundNormalY => _minGroundNormalY;
    }
}
