using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalVarsInitConfigComponent : AAnimalConfigComponent<AnimalVarsInitConfig>
    {
        [InfoBox("Provides the animal's initial variable IDs and values.")]
        [SerializeField, ReadOnly] private bool _about;
    }
}
