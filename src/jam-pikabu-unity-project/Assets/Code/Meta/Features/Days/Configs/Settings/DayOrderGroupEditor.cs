#if UNITY_EDITOR
using System;
using Code.Gameplay.Features.Orders.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs
{
    [Serializable]
    public class DayOrderGroupEditor
    {
        [MinMaxSlider(1, 30, showFields: true)] public Vector2Int Range = Vector2Int.one;
        public OrderTag OrderTag;

        public bool CanUse(int dayId)
        {
            if (dayId > Range.y)
                return false;

            if (dayId < Range.x)
                return false;

            return true;
        }
    }
}
#endif