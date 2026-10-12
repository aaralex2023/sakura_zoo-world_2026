using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.World
{
    [CreateAssetMenu(menuName = "ZooWorld/Settings/Spawn")]
    public sealed class SpawnSettings : ScriptableObject
    {
        [InfoBox("Defines spawn intervals, screen inset, spawn height, and layers that block spawning.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private Vector2 _intervalRange = new(1f, 2f);
        [SerializeField] private float _spawnInset = 1f;
        [SerializeField] private float _spawnHeight = 0.5f;
        [SerializeField] private LayerMask _blockingMask = (1 << 6) | (1 << 8);

        public Vector2 IntervalRange => _intervalRange;
        public float SpawnInset => _spawnInset;
        public float SpawnHeight => _spawnHeight;
        public LayerMask BlockingMask => _blockingMask;
    }
}
