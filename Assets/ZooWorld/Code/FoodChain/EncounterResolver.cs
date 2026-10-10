using System;
using System.Collections.Generic;
using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    public sealed class EncounterResolver : IEncounterResolver
    {
        private readonly IEncounterRule[] _rules;

        public EncounterResolver(List<IEncounterRule> rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            _rules = rules.ToArray();
            for (int i = 0; i < _rules.Length; i++)
            {
                if (_rules[i] == null)
                    throw new ArgumentException("Encounter rules cannot contain null entries.", nameof(rules));
            }
        }

        public EncounterVerdict Resolve(in Encounter encounter)
        {
            foreach (IEncounterRule rule in _rules)
            {
                if (!rule.TryResolve(encounter, out EncounterVerdict verdict))
                    continue;
                if (verdict.Kind == EncounterKind.Bounce)
                    return verdict;
                if (verdict.Kind != EncounterKind.Eat ||
                    !ContainsParticipant(encounter, verdict.Eater) ||
                    !ContainsParticipant(encounter, verdict.Victim) ||
                    ReferenceEquals(verdict.Eater, verdict.Victim))
                    throw new InvalidOperationException($"Encounter rule '{rule.GetType().Name}' returned an invalid verdict.");

                return verdict;
            }

            return EncounterVerdict.Bounce();
        }

        private static bool ContainsParticipant(in Encounter encounter, IAnimal animal)
        {
            return ReferenceEquals(encounter.A, animal) || ReferenceEquals(encounter.B, animal);
        }
    }
}
