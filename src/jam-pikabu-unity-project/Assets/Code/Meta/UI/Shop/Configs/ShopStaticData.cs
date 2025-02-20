using System.Collections.Generic;
using Code.Gameplay.StaticData.Data;
using Code.Meta.Features.Consumables;
using UnityEngine;

namespace Code.Meta.UI.Shop.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(ShopStaticData), fileName = "Shop")]
    public class ShopStaticData : BaseStaticData<ShopItemData>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();

            AddIndex(data => (int)data.ConsumableType);
            AddNonUniqueIndex(data => (int)data.Kind);
        }

        public List<ShopItemData> GetByItemKind(ShopItemKind kind)
        {
            return GetNonUniqueByKey((int)kind);
        }
        
        public ShopItemData GetByConsumableType(ConsumableTypeId consumableType)
        {
            return GetByKey((int)consumableType);
        }
    }
}