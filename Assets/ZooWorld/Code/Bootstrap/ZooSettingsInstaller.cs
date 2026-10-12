using NaughtyAttributes;
using System;
using UnityEngine;
using Zenject;
using ZooWorld.Animals;
using ZooWorld.FoodChain;
using ZooWorld.UI;
using ZooWorld.World;

namespace ZooWorld.Bootstrap
{
    [CreateAssetMenu(menuName = "ZooWorld/Installers/Settings")]
    public sealed class ZooSettingsInstaller : ScriptableObjectInstaller
    {
        [InfoBox("Registers shared spawning, catalog, food-chain, death-counter, and screen-return settings.")]
        [SerializeField, ReadOnly] private bool _about;

        [SerializeField] private SpawnSettings _spawnSettings;
        [SerializeField] private AnimalCatalog _animalCatalog;
        [SerializeField] private FoodChainSettings _foodChainSettings;
        [SerializeField] private DeathCounterSettings _deathCounterSettings;
        [SerializeField] private ScreenReturnConfig _screenReturnConfig = new();

        public override void InstallBindings()
        {
            if (_spawnSettings == null)
                throw new InvalidOperationException($"{nameof(ZooSettingsInstaller)} has no SpawnSettings asset.");
            if (_animalCatalog == null)
                throw new InvalidOperationException($"{nameof(ZooSettingsInstaller)} has no AnimalCatalog asset.");
            if (_foodChainSettings == null)
                throw new InvalidOperationException($"{nameof(ZooSettingsInstaller)} has no FoodChainSettings asset.");
            if (_deathCounterSettings == null)
                throw new InvalidOperationException($"{nameof(ZooSettingsInstaller)} has no DeathCounterSettings asset.");
            if (_screenReturnConfig == null)
                throw new InvalidOperationException($"{nameof(ZooSettingsInstaller)} has no ScreenReturnConfig.");

            Container.BindInstance(_spawnSettings).AsSingle();
            Container.BindInstance(_animalCatalog).AsSingle();
            Container.BindInstance(_foodChainSettings).AsSingle();
            Container.BindInstance(_deathCounterSettings).AsSingle();
            Container.BindInstance(_screenReturnConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<DeathCounterModel>().AsSingle();
        }
    }
}
