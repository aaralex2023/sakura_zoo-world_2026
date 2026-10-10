using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public abstract class AAnimalConfigComponent : MonoBehaviour
    {
        public abstract void GetConfig(out Type type, out IAnimalConfig config);
    }
}
