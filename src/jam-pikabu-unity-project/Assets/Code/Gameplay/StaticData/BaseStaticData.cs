using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.StaticData
{
    public abstract class BaseData
    {
         public int Id;
    }
    
    public abstract class BaseStaticData<T> : BaseStaticData where T : class
    {
        [PropertyOrder(99), ListDrawerSettings(CustomAddFunction = nameof(OnAddNewConfig))] public List<T> Configs;
        
        private readonly Dictionary<int, T> _data = new();

        protected void AddIndex(Func<T, int> keySelector)
        {
            if (Configs == null || keySelector == null)
                throw new ArgumentNullException();

            _data.Clear();

            foreach (var item in Configs)
            {
                int key = keySelector(item);
                _data.TryAdd(key, item);
            }
        }

        protected T GetByKey(int key)
        {
            _data.TryGetValue(key, out var value);
            return value;
        }

        private T OnAddNewConfig()
        {
            var onAddNewConfig = Activator.CreateInstance<T>();
            
            if (onAddNewConfig is BaseData baseData) 
                baseData.Id = Configs.Count;
            
            return onAddNewConfig;
        }
        
    }
}