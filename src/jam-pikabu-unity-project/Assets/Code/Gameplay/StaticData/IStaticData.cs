using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.StaticData
{
    public interface IStaticData
    {
    }

    public abstract class BaseStaticData : ScriptableObject, IStaticData
    {
        public virtual void OnConfigPreInit()
        {
        }

        public virtual void OnConfigInit()
        {
        }
    }

    public abstract class BaseStaticData<T> : ScriptableObject, IStaticData where T : class
    {
        public List<T> Configs;
        
        private readonly Dictionary<int, T> _data = new();

        public void AddIndex(Func<T, int> keySelector)
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

        public T GetByKey(int key)
        {
            _data.TryGetValue(key, out var value);
            return value;
        }

        public virtual void OnConfigPreInit()
        {
        }

        public virtual void OnConfigInit()
        {
        }
    }
}