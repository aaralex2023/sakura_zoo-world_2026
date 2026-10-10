using System;
using ZooWorld.Animals;

namespace ZooWorld.FoodChain
{
    public readonly struct EncounterVerdict
    {
        public readonly EncounterKind Kind;
        public readonly IAnimal Eater;
        public readonly IAnimal Victim;

        private EncounterVerdict(EncounterKind kind, IAnimal eater, IAnimal victim)
        {
            Kind = kind;
            Eater = eater;
            Victim = victim;
        }

        public static EncounterVerdict Bounce()
        {
            return new EncounterVerdict(EncounterKind.Bounce, null, null);
        }

        public static EncounterVerdict Eat(IAnimal eater, IAnimal victim)
        {
            if (eater == null)
                throw new ArgumentNullException(nameof(eater));
            if (victim == null)
                throw new ArgumentNullException(nameof(victim));
            if (ReferenceEquals(eater, victim))
                throw new ArgumentException("An animal cannot eat itself.", nameof(victim));

            return new EncounterVerdict(EncounterKind.Eat, eater, victim);
        }
    }
}
