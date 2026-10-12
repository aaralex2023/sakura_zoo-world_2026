using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class TastyLabelConfigComponent : AAnimalConfigComponent<TastyLabelConfig>
    {
        [InfoBox("Provides the text, duration, and screen offset for labels shown after eating.")]
        [SerializeField, ReadOnly] private bool _about;
    }
}
