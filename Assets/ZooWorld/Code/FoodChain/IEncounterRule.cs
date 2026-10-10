namespace ZooWorld.FoodChain
{
    public interface IEncounterRule
    {
        bool TryResolve(in Encounter encounter, out EncounterVerdict verdict);
    }
}
