using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    public sealed class SingleEaterRule : IEncounterRule
    {
        public bool TryResolve(in Encounter encounter, out EncounterVerdict verdict)
        {
            bool aCanEat = EncounterRoleUtility.GetRole(encounter.A).CanEat;
            bool bCanEat = EncounterRoleUtility.GetRole(encounter.B).CanEat;
            if (aCanEat == bCanEat)
            {
                verdict = default;
                return false;
            }

            verdict = aCanEat
                ? EncounterVerdict.Eat(encounter.A, encounter.B)
                : EncounterVerdict.Eat(encounter.B, encounter.A);
            return true;
        }
    }
}
