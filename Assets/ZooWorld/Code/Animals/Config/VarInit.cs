using UnityEngine;

namespace ZooWorld.Animals
{
    [System.Serializable]
    public struct VarInit
    {
        [SerializeField] private AnimalVarId _id;
        [SerializeField] private double _value;

        public AnimalVarId Id => _id;
        public double Value => _value;
    }
}
