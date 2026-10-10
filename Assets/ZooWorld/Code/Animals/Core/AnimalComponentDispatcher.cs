using System;
using UnityEngine;

namespace ZooWorld.Animals
{
    public sealed class AnimalComponentDispatcher : MonoBehaviour, IAnimalComponentDispatcher
    {
        [SerializeField] private GameObject _root;

        private IAnimalContextUser[] _contextUsers = Array.Empty<IAnimalContextUser>();
        private IAnimalAteListener[] _ateListeners = Array.Empty<IAnimalAteListener>();
        private IAnimalEatenListener[] _eatenListeners = Array.Empty<IAnimalEatenListener>();
        private bool _isInitialized;

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
