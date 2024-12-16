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

            AddIndex(data => (int)data.Kind);
        }

        public ShopItemData GetByItemKind(ShopItemKind kind)
        {
            return GetByKey((int)kind);
        }
    }
}