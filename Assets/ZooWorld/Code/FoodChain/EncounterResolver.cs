using System;
using ZooWorld.Animals;
using ZooWorld.World;

namespace ZooWorld.FoodChain
{
    public sealed class EncounterResolver : IEncounterResolver
    {
        private readonly AnimalVarId _rankVar;
        private readonly IRandom _random;

        public EncounterResolver(FoodChainSettings settings, IRandom random)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            _rankVar = settings.RankVar != null
                ? settings.RankVar
                : throw new ArgumentException($"{nameof(FoodChainSettings)} has no rank variable.", nameof(settings));
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public bool TryResolve(IAnimal a, IAnimal b, out IAnimal eater, out IAnimal victim)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));
            if (ReferenceEquals(a, b))
                throw new ArgumentException("An encounter requires two different animals.", nameof(b));

            FoodChainRoleId aRole = GetRole(a);
            FoodChainRoleId bRole = GetRole(b);
            if (!aRole.CanEat && !bRole.CanEat)
            {
                eater = null;
                victim = null;
                return false;
            }

            if (aRole.CanEat != bRole.CanEat)
            {
                eater = aRole.CanEat ? a : b;
                victim = aRole.CanEat ? b : a;
                return true;
            }

            double aRank = Math.Round(a.Vars.Get(_rankVar));
            double bRank = Math.Round(b.Vars.Get(_rankVar));
            bool aWins = aRank == bRank ? _random.Chance(0.5f) : aRank > bRank;
            eater = aWins ? a : b;
            victim = aWins ? b : a;
            return true;
        }

        private static FoodChainRoleId GetRole(IAnimal animal)
        {
            if (!animal.Configs.TryGet(out FoodChainConfig config) || config.Role == null)
                throw new InvalidOperationException($"Animal '{animal.Transform.name}' has no food-chain role.");

            return config.Role;
        }
    }
}
