using NaughtyAttributes;
using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalComponentDispatcher : MonoBehaviour, IAnimalComponentDispatcher
    {
        [InfoBox("Initializes reactors and forwards eating and being-eaten events.\nAssign a reactor root; place enabled reactors on it or its active direct children.\nDeeper descendants are not searched.\nAssign to the animal; use a dedicated reactor root directly beneath the animal.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private GameObject _root;

        private IAnimalContextUser[] _contextUsers = Array.Empty<IAnimalContextUser>();
        private IAnimalAteListener[] _ateListeners = Array.Empty<IAnimalAteListener>();
        private IAnimalEatenListener[] _eatenListeners = Array.Empty<IAnimalEatenListener>();
        private bool _isInitialized;

        public GameObject ReactorsRoot => _root;

        public void Init(AnimalContext context)
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(AnimalComponentDispatcher)} on '{name}' was initialized more than once.");
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (_root == null)
                throw new InvalidOperationException($"'{name}' is missing its reactor root.");

            _contextUsers = ComponentExtractorUtil.ExtractDirectChildren<IAnimalContextUser>(_root);
            _ateListeners = ComponentExtractorUtil.ExtractDirectChildren<IAnimalAteListener>(_root);
            _eatenListeners = ComponentExtractorUtil.ExtractDirectChildren<IAnimalEatenListener>(_root);

            foreach (IAnimalContextUser contextUser in _contextUsers)
                contextUser.SetupContext(context);

            _isInitialized = true;
        }

        public void Ate(IAnimal victim)
        {
            EnsureInitialized();
            foreach (IAnimalAteListener listener in _ateListeners)
                listener.OnAte(victim);
        }

        public void Eaten(IAnimal eater)
        {
            EnsureInitialized();
            foreach (IAnimalEatenListener listener in _eatenListeners)
                listener.OnEaten(eater);
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException($"'{name}' has not been initialized.");
        }
    }
}
