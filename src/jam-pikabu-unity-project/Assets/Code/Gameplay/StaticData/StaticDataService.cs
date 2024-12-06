using System;
using System.Collections.Generic;
using Code.Common.Logger.Service;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly ILoggerService _loggerService;
        private readonly LazyInject<List<IConfigsInitHandler>> _handlers;

        private readonly Dictionary<Type, BaseStaticData> _configs = new();

        public StaticDataService(ILoggerService loggerService,
            LazyInject<List<IConfigsInitHandler>> handlers)
        {
            _loggerService = loggerService;
            _handlers = handlers;
        }

        public void Load()
        {
            BuildConfigStaticData buildConfig = Resources.Load<BuildConfigStaticData>("Configs/BuildConfig");

            BaseStaticData[] configs;
            switch (buildConfig.ConfigType)
            {
                case BuildConfigType.Dev:
                    configs = Resources.LoadAll<BaseStaticData>("Configs/Dev");
                    break;
                case BuildConfigType.Prod:
                    configs = Resources.LoadAll<BaseStaticData>("Configs/Prod");
                    break;
                default:
                {
                    _loggerService.LogError($"Unknown build config type: {buildConfig.ConfigType}");
                    throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var config in configs)
            {
                _configs[config.GetType()] = config;
                config.OnConfigPreInit();
            }

            foreach (var config in configs)
            {
                config.OnConfigInit();
            }

            NotifyHandlers();
        }

        public T GetStaticData<T>() where T : class
        {
            return _configs[typeof(T)] as T;
        }

        private void NotifyHandlers()
        {
            foreach (var handler in _handlers.Value)
            {
                handler.OnConfigsInitInitComplete();
            }
        }
    }
}