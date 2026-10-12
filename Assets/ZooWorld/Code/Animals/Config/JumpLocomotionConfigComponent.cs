using NaughtyAttributes;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class JumpLocomotionConfigComponent : AAnimalConfigComponent<JumpLocomotionConfig>
    {
        [InfoBox("Provides the jump distance and air time used by jumping states.")]
        [SerializeField, ReadOnly] private bool _about;
    }
}
