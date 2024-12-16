using Code.Gameplay.StaticData.Data;
using Code.Meta.UI.Shop.Items;
using UnityEngine;

namespace Code.Meta.UI.Shop.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(ShopItemTemplatesData), fileName = "ShopItemTemplates")]
    public class ShopItemTemplatesData : BaseStaticData<ShopItemTemplateData>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();

            AddIndex(data => (int)data.Kind);
        }

        public ShopItemTemplateData GetByItemKind(ShopItemKind kind)
        {
            return GetByKey((int)kind);
        }
    }
}