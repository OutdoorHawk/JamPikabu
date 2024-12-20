using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Code.Gameplay.StaticData.Data
{
    public abstract class BaseStaticData<TData> : BaseStaticData where TData : BaseData
    {
        [PropertyOrder(99)] public List<TData> Configs;

        private Dictionary<int, TData> _uniqueIndex;
        private Dictionary<int, List<TData>> _nonUniqueIndex;

        protected void AddIndex(Func<TData, int> keySelector)
        {
            if (Configs == null || keySelector == null)
                throw new ArgumentNullException();

            _uniqueIndex = new Dictionary<int, TData>();

            foreach (var item in Configs)
            {
                int key = keySelector(item);
                _uniqueIndex.TryAdd(key, item);
            }
        }
        
        protected void AddNonUniqueIndex(Func<TData, int> keySelector)
        {
            if (Configs == null || keySelector == null)
                throw new ArgumentNullException();

            _nonUniqueIndex = new Dictionary<int, List<TData>>();

            foreach (var item in Configs)
            {
                int key = keySelector(item);
                
                if (_nonUniqueIndex.ContainsKey(key))
                    _nonUniqueIndex[key].Add(item);
                else
                    _nonUniqueIndex.Add(key, new List<TData> { item });
            }
        }

        protected TData GetByKey(int key)
        {
            _uniqueIndex.TryGetValue(key, out var value);
            return value;
        }
        
        protected List<TData> GetNonUniqueByKey(int key)
        {
            _nonUniqueIndex.TryGetValue(key, out var value);
            return value;
        }

        private void OnValidate()
        {
            for (int i = 0; i < Configs.Count; i++)
                Configs[i].Id = i + 1;
        }
    }
}