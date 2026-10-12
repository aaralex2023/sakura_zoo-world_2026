using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    [CreateAssetMenu(menuName = "ZooWorld/Ids/Var")]
    public sealed class AnimalVarId : ScriptableObject
    {
        [InfoBox("Identifies an animal variable used by initial values, timers, flags, or rank tracking.")]
        [SerializeField, ReadOnly] private bool _about;
    }
}
