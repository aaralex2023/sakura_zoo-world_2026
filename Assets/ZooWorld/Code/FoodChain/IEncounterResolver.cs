namespace ZooWorld.FoodChain
{
    public interface IEncounterResolver
    {
        EncounterVerdict Resolve(in Encounter encounter);
    }
}
