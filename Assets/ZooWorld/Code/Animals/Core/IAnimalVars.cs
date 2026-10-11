namespace ZooWorld.Animals
{
    public interface IAnimalVars
    {
        double Get(AnimalVarId id);
        bool TryGet(AnimalVarId id, out double value);
        void Set(AnimalVarId id, double value);
        bool Remove(AnimalVarId id);
        void Add(AnimalVarId id, double delta);
        bool Compare(AnimalVarId id, double other, VarComparison comparison);
    }
}
