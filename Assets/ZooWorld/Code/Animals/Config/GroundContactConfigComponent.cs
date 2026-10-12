using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class GroundContactConfigComponent : AAnimalConfigComponent<GroundContactConfig>
    {
        [InfoBox("Provides the ground layer mask and minimum upward contact normal used to detect grounding.")]
        [SerializeField, ReadOnly] private bool _about;
    }
}
