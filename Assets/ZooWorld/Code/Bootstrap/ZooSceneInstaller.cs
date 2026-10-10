using System;
using UnityEngine;
using Zenject;
using ZooWorld.FoodChain;
using ZooWorld.World;

namespace ZooWorld.Bootstrap
{
    public sealed class ZooSceneInstaller : MonoInstaller
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _animalsRoot;

        public override void InstallBindings()
        {
            if (_camera == null)
                throw new InvalidOperationException($"{nameof(ZooSceneInstaller)} has no camera reference.");
            if (_animalsRoot == null)
                throw new InvalidOperationException($"{nameof(ZooSceneInstaller)} has no animals root reference.");

            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<AnimalDiedSignal>();

            Container.Bind<IRandom>().To<UnityRandom>().AsSingle();
            Container.Bind<IEncounterRule>().To<NoEatersRule>().AsSingle();
            Container.Bind<IEncounterRule>().To<SingleEaterRule>().AsSingle();
            Container.Bind<IEncounterRule>().To<RankDuelRule>().AsSingle();
            Container.Bind<IEncounterResolver>().To<EncounterResolver>().AsSingle();
            Container.BindInterfacesTo<CameraScreenBounds>().AsSingle().WithArguments(_camera);
            Container.Bind<IAnimalFactory>().To<AnimalFactory>().AsSingle().WithArguments(_animalsRoot);
            Container.Bind<IAnimalDespawner>().To<DestroyAnimalDespawner>().AsSingle();
            Container.BindInterfacesTo<AnimalSpawner>().AsSingle();
        }
    }
}
