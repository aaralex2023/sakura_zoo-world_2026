using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    [CreateAssetMenu(menuName = "ZooWorld/Ids/State")]
    public sealed class AnimalStateId : ScriptableObject
    {
        [InfoBox("Identifies a state shared by state nodes and transition targets.")]
        [SerializeField, ReadOnly] private bool _about;
    }
}
