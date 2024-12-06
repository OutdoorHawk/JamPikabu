using System;
using System.Collections.Generic;
using Code.Common.Logger.Service;
using UnityEngine;

namespace Code.Gameplay.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly ILoggerService _loggerService;

        private readonly Dictionary<Type, BaseStaticData> _configs = new();

        public StaticDataService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
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
        }

        public T GetStaticData<T>() where T : class
        {
            return _configs[typeof(T)] as T;
        }
    }
}