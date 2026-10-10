using System.Collections.Generic;
using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public sealed class AnimalVarsInitConfig : IAnimalConfig
    {
        [SerializeField] private List<VarInit> _values = new();
        public IReadOnlyList<VarInit> Values => _values;
    }
}
