using System;
using System.Collections.Generic;
using TMPro;
using Zenject;

namespace ZooWorld.UI
{
    public sealed class DeathCounterPresenter : IInitializable, IDisposable
    {
        private readonly DeathCounterModel _model;
        private readonly DeathCounterSettings _settings;
        private readonly DeathCounterView _view;
        private readonly TMP_FontAsset _font;
        private bool _isInitialized;

        public DeathCounterPresenter(
            DeathCounterModel model,
            DeathCounterSettings settings,
            DeathCounterView view,
            TMP_FontAsset font)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _settings = settings != null ? settings : throw new ArgumentNullException(nameof(settings));
            _view = view != null ? view : throw new ArgumentNullException(nameof(view));
            _font = font != null ? font : throw new ArgumentNullException(nameof(font));
        }

        public void Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(DeathCounterPresenter)} was initialized more than once.");
            if (string.IsNullOrWhiteSpace(_settings.Format))
                throw new InvalidOperationException($"{nameof(DeathCounterSettings)} has no count format.");
            if (_settings.Roles.Count == 0)
                throw new InvalidOperationException($"{nameof(DeathCounterSettings)} has no roles.");

            HashSet<FoodChainRoleId> roles = new();
            foreach (FoodChainRoleId role in _settings.Roles)
            {
                if (role == null)
                    throw new InvalidOperationException($"{nameof(DeathCounterSettings)} contains a null role.");
                if (!roles.Add(role))
                    throw new InvalidOperationException($"{nameof(DeathCounterSettings)} contains duplicate role '{role.name}'.");
            }

            _view.Initialize(_settings.Roles, _font);
            _model.Changed += OnChanged;
            _isInitialized = true;

            foreach (FoodChainRoleId role in _settings.Roles)
                Present(role, _model.Get(role));
        }

        public void Dispose()
        {
            if (!_isInitialized)
                return;

            _model.Changed -= OnChanged;
            _isInitialized = false;
        }

        private void OnChanged(FoodChainRoleId role, int count)
        {
            Present(role, count);
        }

        private void Present(FoodChainRoleId role, int count)
        {
            _view.SetCount(role, string.Format(_settings.Format, role.DisplayName, count));
        }
    }
}
