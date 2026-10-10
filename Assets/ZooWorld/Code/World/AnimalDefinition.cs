using UnityEngine;

namespace ZooWorld.World
{
    [CreateAssetMenu(menuName = "ZooWorld/Animal Definition")]
    public sealed class AnimalDefinition : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField, Min(0f)] private float _spawnWeight = 1f;
        [SerializeField, Min(0.01f)] private float _spawnRadius = 1f;

        public GameObject Prefab => _prefab;
        public float SpawnWeight => _spawnWeight;
        public float SpawnRadius => _spawnRadius;
    }
}
