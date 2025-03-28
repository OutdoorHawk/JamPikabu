﻿using System;
using System.Collections.Generic;
using Code.Common.Logger.Service;
using Code.Infrastructure.AssetManagement.AssetProvider;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly LazyInject<List<IConfigsInitHandler>> _handlers;
        private readonly ILoggerService _loggerService;
        private readonly IAssetProvider _assetProvider;

        private readonly Dictionary<Type, BaseStaticData> _configs = new();
        private readonly DiContainer _diContainer;

        private const string BUILD_CONFIG = "BuildConfig";

        public StaticDataService
        (
            ILoggerService loggerService,
            LazyInject<List<IConfigsInitHandler>> handlers,
            IAssetProvider assetProvider,
            DiContainer diContainer
        )
        {
            _diContainer = diContainer;
            _loggerService = loggerService;
            _handlers = handlers;
            _assetProvider = assetProvider;
        }

        public async UniTask Load()
        {
            BuildConfigStaticData buildConfig = await _assetProvider.LoadAssetAsync<BuildConfigStaticData>(BUILD_CONFIG);

            string label = "ProdStaticData";
#if UNITY_EDITOR
            switch (buildConfig.ConfigType)
            {
                case BuildConfigType.Dev:
                    label = "DevStaticData"; // Уникальный label для dev-конфигурации
                    break;
                case BuildConfigType.Prod:
                    label = "ProdStaticData"; // Уникальный label для prod-конфигурации
                    break;
                default:
                    _loggerService.LogError($"Unknown build config type: {buildConfig.ConfigType}");
                    throw new ArgumentOutOfRangeException();
            }
#endif
            
            IList<BaseStaticData> result = await _assetProvider.LoadAssetsAsync<BaseStaticData>(label);

            if (result == null)
            {
                _loggerService.LogError($"Error loading data by label: {label}");
                return;
            }
            
            _configs[buildConfig.GetType()] = buildConfig;

            foreach (var config in result)
            {
                _diContainer.Inject(config);
            }

            foreach (var config in result)
            {
                _configs[config.GetType()] = config;
                config.OnConfigPreInit();
            }

            foreach (var config in result)
            {
                config.OnConfigInit();
            }

            NotifyHandlers();
        }

        public T Get<T>() where T : class
        {
            return _configs[typeof(T)] as T;
        }

        public void RegisterHandler(IConfigsInitHandler handler)
        {
            _handlers.Value.Add(handler);
        }
        
        public void UnRegisterHandler(IConfigsInitHandler handler)
        {
            _handlers.Value.Remove(handler);
        }

        private void NotifyHandlers()
        {
            foreach (var handler in _handlers.Value)
            {
                handler.OnConfigsInitComplete();
            }
        }
    }
}