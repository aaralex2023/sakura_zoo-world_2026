using System;
using UnityEngine;
using Zenject;
using ZooWorld.Animals;
using ZooWorld.World;

namespace ZooWorld.FoodChain
{
    public sealed class AnimalCollisionRouter : MonoBehaviour, IAnimalContextUser
    {
        private IAnimal _animal;
        private IEncounterResolver _resolver;
        private IAnimalDespawner _despawner;

        [Inject]
        private void Construct(IEncounterResolver resolver, IAnimalDespawner despawner)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _despawner = despawner ?? throw new ArgumentNullException(nameof(despawner));
        }

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_animal != null)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_resolver == null || _despawner == null)
                throw new InvalidOperationException($"'{name}' has no injected encounter resolver or despawner.");

            _animal = context.Animal;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_animal == null || _animal.IsDead || collision == null || collision.rigidbody == null)
                return;
            if (!collision.rigidbody.TryGetComponent(out Animal other) || ReferenceEquals(_animal, other))
                return;
            if (other.IsDead || _animal.Body.GetInstanceID() > other.Body.GetInstanceID())
                return;

            Encounter encounter = new(_animal, other);
            EncounterVerdict verdict = _resolver.Resolve(encounter);
            if (verdict.Kind != EncounterKind.Eat || !verdict.Victim.TryDie(verdict.Eater))
                return;

            verdict.Eater.NotifyAte(verdict.Victim);
            verdict.Victim.NotifyEaten(verdict.Eater);
            _despawner.Despawn(verdict.Victim);
        }
    }
}
