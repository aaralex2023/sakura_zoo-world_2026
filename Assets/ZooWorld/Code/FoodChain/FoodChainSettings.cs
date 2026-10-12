using NaughtyAttributes;
using UnityEngine;
using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    [CreateAssetMenu(menuName = "ZooWorld/Settings/Food Chain")]
    public sealed class FoodChainSettings : ScriptableObject
    {
        [InfoBox("Selects the rank variable used to compare animals that can both eat.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private AnimalVarId _rankVar;

        public AnimalVarId RankVar => _rankVar;
    }
}
