using UnityEngine;
using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    [CreateAssetMenu(menuName = "ZooWorld/Settings/Food Chain")]
    public sealed class FoodChainSettings : ScriptableObject
    {
        [SerializeField] private AnimalVarId _rankVar;

        public AnimalVarId RankVar => _rankVar;
    }
}
