using System;
using TMPro;
using UnityEngine;
using Zenject;
using ZooWorld.Animals;
using ZooWorld.FoodChain;
using ZooWorld.UI;
using ZooWorld.World;

namespace ZooWorld.Bootstrap
{
    public sealed class ZooSceneInstaller : MonoInstaller
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _animalsRoot;
        [SerializeField] private DeathCounterView _deathCounterView;
        [SerializeField] private Canvas _labelsCanvas;

        public override void InstallBindings()
        {
            if (_camera == null)
                throw new InvalidOperationException($"{nameof(ZooSceneInstaller)} has no camera reference.");
            if (_animalsRoot == null)
                throw new InvalidOperationException($"{nameof(ZooSceneInstaller)} has no animals root reference.");
            if (_deathCounterView == null)
                _deathCounterView = FindFirstObjectByType<DeathCounterView>();
            if (_deathCounterView == null)
                throw new InvalidOperationException($"{nameof(ZooSceneInstaller)} has no death counter view reference.");
            if (_labelsCanvas == null)
                throw new InvalidOperationException($"{nameof(ZooSceneInstaller)} has no floating labels canvas reference.");

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<AnimalDiedSignal>();

            Container.Bind<IRandom>().To<UnityRandom>().AsSingle();
            Container.Bind<AnimalHeadingSelector>().AsSingle();
            Container.Bind<IEncounterResolver>().To<EncounterResolver>().AsSingle();
            Container.BindInterfacesTo<CameraScreenBounds>().AsSingle().WithArguments(_camera);
            Container.Bind<IAnimalFactory>().To<AnimalFactory>().AsSingle().WithArguments(_animalsRoot);
            Container.Bind<IAnimalDespawner>().To<DestroyAnimalDespawner>().AsSingle();
            Container.BindInterfacesTo<AnimalSpawner>().AsSingle();
            TMP_FontAsset font = CreateUiFont();
            Container.BindInstance(font).AsSingle();
            Container.BindInstance(_deathCounterView).AsSingle();
            Container.BindInterfacesTo<DeathCounterPresenter>().AsSingle();
            Container.Bind<IFloatingLabelService>().To<FloatingLabelService>().AsSingle()
                .WithArguments(_labelsCanvas, _camera, font);
        }

        private static TMP_FontAsset CreateUiFont()
        {
            TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font == null)
                throw new InvalidOperationException("Could not load the TextMeshPro default font asset.");
            return font;
        }
    }
}
