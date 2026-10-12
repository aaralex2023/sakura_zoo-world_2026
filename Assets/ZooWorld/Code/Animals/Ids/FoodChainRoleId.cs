using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    [CreateAssetMenu(menuName = "ZooWorld/Ids/FoodChainRole")]
    public sealed class FoodChainRoleId : ScriptableObject
    {
        [InfoBox("Defines a food-chain role's display name and whether animals with this role can eat.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private string _displayName;
        [SerializeField] private bool _canEat;

        public string DisplayName => _displayName;
        public bool CanEat => _canEat;
    }
}
