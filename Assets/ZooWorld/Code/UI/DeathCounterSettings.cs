using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Animals;

namespace ZooWorld.UI
{
    [CreateAssetMenu(menuName = "ZooWorld/Settings/Death Counter")]
    public sealed class DeathCounterSettings : ScriptableObject
    {
        [InfoBox("Selects the food-chain roles displayed by death counters and their text format.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private List<FoodChainRoleId> _roles = new();
        [SerializeField] private string _format = "{0}: {1}";

        public IReadOnlyList<FoodChainRoleId> Roles => _roles;
        public string Format => _format;
    }
}
