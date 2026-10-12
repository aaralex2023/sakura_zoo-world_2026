using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.World
{
    [CreateAssetMenu(menuName = "ZooWorld/Animal Definition")]
    public sealed class AnimalDefinition : ScriptableObject
    {
        [InfoBox("Defines an animal prefab, its relative spawn weight, and the radius used to check spawn clearance.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private GameObject _prefab;
        [SerializeField, Min(0f)] private float _spawnWeight = 1f;
        [SerializeField, Min(0.01f)] private float _spawnRadius = 1f;

        public GameObject Prefab => _prefab;
        public float SpawnWeight => _spawnWeight;
        public float SpawnRadius => _spawnRadius;
    }
}
