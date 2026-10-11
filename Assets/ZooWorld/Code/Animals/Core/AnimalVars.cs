using System.Collections.Generic;

namespace ZooWorld.Animals
{
    public sealed class AnimalVars : IAnimalVars
    {
        private const double Epsilon = 1e-9;
        private readonly Dictionary<AnimalVarId, double> _values = new();

        public double Get(AnimalVarId id)
        {
            return TryGet(id, out double value) ? value : 0d;
        }

        public bool TryGet(AnimalVarId id, out double value)
        {
            return _values.TryGetValue(id, out value);
        }

        public void Set(AnimalVarId id, double value)
        {
            _values[id] = value;
        }

        public bool Remove(AnimalVarId id)
        {
            return _values.Remove(id);
        }

        public void Add(AnimalVarId id, double delta)
        {
            Set(id, Get(id) + delta);
        }

        public bool Compare(AnimalVarId id, double other, VarComparison comparison)
        {
            double value = Get(id);
            return comparison switch
            {
                VarComparison.Equal => System.Math.Abs(value - other) <= Epsilon,
                VarComparison.NotEqual => System.Math.Abs(value - other) > Epsilon,
                VarComparison.Greater => value > other + Epsilon,
                VarComparison.Less => value < other - Epsilon,
                VarComparison.GreaterOrEqual => value >= other - Epsilon,
                VarComparison.LessOrEqual => value <= other + Epsilon,
                _ => throw new System.ArgumentOutOfRangeException(nameof(comparison), comparison, null)
            };
        }
    }
}
