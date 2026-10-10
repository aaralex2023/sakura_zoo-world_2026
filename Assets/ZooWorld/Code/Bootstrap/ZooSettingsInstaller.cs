using System;
using UnityEngine;
using Zenject;
using ZooWorld.World;

namespace ZooWorld.Bootstrap
{
    [CreateAssetMenu(menuName = "ZooWorld/Installers/Settings")]
    public sealed class ZooSettingsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private SpawnSettings _spawnSettings;
        [SerializeField] private AnimalCatalog _animalCatalog;

        public override void InstallBindings()
        {
            if (_spawnSettings == null)
                throw new InvalidOperationException($"{nameof(ZooSettingsInstaller)} has no SpawnSettings asset.");
            if (_animalCatalog == null)
                throw new InvalidOperationException($"{nameof(ZooSettingsInstaller)} has no AnimalCatalog asset.");

            Container.BindInstance(_spawnSettings).AsSingle();
            Container.BindInstance(_animalCatalog).AsSingle();
        }
    }
}
