using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class GroundContactConfig : IAnimalConfig
    {
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _minGroundNormalY;

        public LayerMask GroundMask => _groundMask;
        public float MinGroundNormalY => _minGroundNormalY;
    }
}
