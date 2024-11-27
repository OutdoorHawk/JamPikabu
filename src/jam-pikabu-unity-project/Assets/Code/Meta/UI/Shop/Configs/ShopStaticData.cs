using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.UI.Shop.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(ShopStaticData), fileName = "Shop")]
    public class ShopStaticData : ScriptableObject
    {
        [
            TabGroup("Shop"),
            SerializeField,
            ListDrawerSettings
            (
                ShowIndexLabels = true
            )
        ]
        public List<ShopItemConfig> Configs;
    }
}