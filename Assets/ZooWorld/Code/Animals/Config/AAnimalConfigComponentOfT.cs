using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public abstract class AAnimalConfigComponent<T> : AAnimalConfigComponent
        where T : class, IAnimalConfig
    {
        [SerializeField] protected T _config;

        public override void GetConfig(out Type type, out IAnimalConfig config)
        {
            type = typeof(T);
            config = _config;
        }
    }
}
