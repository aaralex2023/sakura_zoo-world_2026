using UnityEngine;

namespace ZooWorld.Animals
{
    [CreateAssetMenu(menuName = "ZooWorld/Ids/FoodChainRole")]
    public sealed class FoodChainRoleId : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private bool _canEat;

        public string DisplayName => _displayName;
        public bool CanEat => _canEat;
    }
}
