using System;
using ZooWorld.Animals;
using ZooWorld.World;

namespace ZooWorld.FoodChain
{
    public sealed class RankDuelRule : IEncounterRule
    {
        private readonly AnimalVarId _rankVar;
        private readonly IRandom _random;

        public RankDuelRule(FoodChainSettings settings, IRandom random)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            _rankVar = settings.RankVar != null
                ? settings.RankVar
                : throw new ArgumentException($"{nameof(FoodChainSettings)} has no rank variable.", nameof(settings));
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public bool TryResolve(in Encounter encounter, out EncounterVerdict verdict)
        {
            FoodChainRoleId aRole = EncounterRoleUtility.GetRole(encounter.A);
            FoodChainRoleId bRole = EncounterRoleUtility.GetRole(encounter.B);
            if (!aRole.CanEat || !bRole.CanEat)
            {
                verdict = default;
                return false;
            }

            double aRank = Math.Round(encounter.A.Vars.Get(_rankVar));
            double bRank = Math.Round(encounter.B.Vars.Get(_rankVar));
            if (aRank == bRank)
            {
                verdict = _random.Chance(0.5f)
                    ? EncounterVerdict.Eat(encounter.A, encounter.B)
                    : EncounterVerdict.Eat(encounter.B, encounter.A);
            }
            else
            {
                verdict = aRank > bRank
                    ? EncounterVerdict.Eat(encounter.A, encounter.B)
                    : EncounterVerdict.Eat(encounter.B, encounter.A);
            }

            return true;
        }
    }
}
