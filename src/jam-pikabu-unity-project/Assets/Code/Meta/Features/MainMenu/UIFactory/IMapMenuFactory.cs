using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.MapBlocks.Behaviours;
using UnityEngine;

namespace Code.Meta.Features.MainMenu.UIFactory
{
    public interface IMapMenuFactory
    {
        MapBlock CreateMapBlock(MapBlockData mapBlockData, RectTransform parent);
    }
}