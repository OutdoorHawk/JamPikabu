using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.MapBlocks.Behaviours;
using UnityEngine;
using Zenject;

namespace Code.Meta.Features.MainMenu.UIFactory
{
    public class MapMenuFactory : IMapMenuFactory
    {
        private readonly IInstantiator _instantiator;

        public MapMenuFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public MapBlock CreateMapBlock(MapBlockData mapBlockData, RectTransform parent)
        {
            MapBlock mapBlock = _instantiator.InstantiatePrefabForComponent<MapBlock>(mapBlockData.ViewPrefab, parent);
            mapBlock.InitData(mapBlockData);
            return mapBlock;
        }
    }
}