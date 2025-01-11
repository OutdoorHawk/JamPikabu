using System.Collections.Generic;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.UI.Shop.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(ShopStaticData), fileName = "Shop")]
    public class ShopStaticData : BaseStaticData<ShopItemData>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();

            AddNonUniqueIndex(data => (int)data.Kind);
        }

        public List<ShopItemData> GetByItemKind(ShopItemKind kind)
        {
            return GetNonUniqueByKey((int)kind);
        }
    }
}