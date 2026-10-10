using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    public sealed class NoEatersRule : IEncounterRule
    {
        public bool TryResolve(in Encounter encounter, out EncounterVerdict verdict)
        {
            if (EncounterRoleUtility.GetRole(encounter.A).CanEat || EncounterRoleUtility.GetRole(encounter.B).CanEat)
            {
                verdict = default;
                return false;
            }

            verdict = EncounterVerdict.Bounce();
            return true;
        }
    }
}
