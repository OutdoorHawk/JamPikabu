using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Code.Gameplay.StaticData.Data
{
    public abstract class BaseStaticData<T> : BaseStaticData where T : BaseData
    {
        [PropertyOrder(99)] public List<T> Configs;

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

        private void OnValidate()
        {
            for (int i = 0; i < Configs.Count; i++)
                Configs[i].Id = i + 1;
        }
    }
}