using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class CrawlLocomotionConfigComponent : AAnimalConfigComponent<CrawlLocomotionConfig>
    {
        [InfoBox("Provides the movement speed used by crawling states.")]
        [SerializeField, ReadOnly] private bool _about;
    }
}
