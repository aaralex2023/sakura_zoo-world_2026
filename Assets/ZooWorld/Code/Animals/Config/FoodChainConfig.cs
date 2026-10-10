using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class FoodChainConfig : IAnimalConfig
    {
        [SerializeField] private FoodChainRoleId _role;
        public FoodChainRoleId Role => _role;
    }
}
