using NaughtyAttributes;
using System;
using UnityEngine;
using Zenject;
using ZooWorld.UI;

namespace ZooWorld.Animals
{
    public sealed class AnimalShowLabelOnAte : MonoBehaviour, IAnimalContextUser, IAnimalAteListener
    {
        [InfoBox("Shows the configured floating label at the assigned anchor whenever this animal eats another animal.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private Transform _labelAnchor;

        private IFloatingLabelService _labelService;
        private TastyLabelConfig _config;
        private bool _isInitialized;

        [Inject]
        private void Construct(IFloatingLabelService labelService)
        {
            _labelService = labelService ?? throw new ArgumentNullException(nameof(labelService));
        }

        public void SetupContext(AnimalContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_isInitialized)
                throw new InvalidOperationException($"'{name}' received animal context more than once.");
            if (_labelService == null)
                throw new InvalidOperationException($"'{name}' has no injected floating label service.");
            if (_labelAnchor == null)
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no label anchor.");
            if (!context.Configs.TryGet(out _config))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has no TastyLabelConfig.");
            if (string.IsNullOrWhiteSpace(_config.Text))
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has an empty tasty label.");
            if (float.IsNaN(_config.Duration) || float.IsInfinity(_config.Duration) || _config.Duration <= 0f)
                throw new InvalidOperationException($"Animal '{context.Animal.Transform.name}' has an invalid tasty label duration.");

            _isInitialized = true;
        }

        public void OnAte(IAnimal victim)
        {
            if (!_isInitialized)
                throw new InvalidOperationException($"'{name}' has not received animal context.");
            if (victim == null)
                throw new ArgumentNullException(nameof(victim));

            _labelService.Show(_config.Text, _labelAnchor, _config.ScreenOffset, _config.Duration);
        }
    }
}
