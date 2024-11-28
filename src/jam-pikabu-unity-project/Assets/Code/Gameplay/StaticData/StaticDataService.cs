using System;
using System.Collections.Generic;
using Code.Infrastructure.AssetManagement.AssetProvider;
using UnityEngine;

namespace Code.Gameplay.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly IAssetProvider _assetProvider;

        private readonly Dictionary<Type, IStaticData> _configs = new();

        public StaticDataService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public void Load()
        {
            BaseStaticData[] configs = Resources.LoadAll<BaseStaticData>("Configs");

            foreach (var config in configs)
            {
                _configs[config.GetType()] = config;
                config.OnConfigPreInit();
            }
            
            foreach (var config in configs)
            {
                config.OnConfigInit();
            }
        }

        public T GetStaticData<T>() where T : class, IStaticData
        {
            return _configs[typeof(T)] as T;
        }
    }
}