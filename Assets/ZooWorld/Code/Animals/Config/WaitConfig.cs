using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class WaitConfig : IAnimalConfig
    {
        [SerializeField] private float _duration;

        public float Duration => _duration;
    }
}
