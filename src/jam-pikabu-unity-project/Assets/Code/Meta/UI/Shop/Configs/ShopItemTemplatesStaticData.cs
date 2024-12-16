using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.UI.Shop.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(ShopItemTemplatesStaticData), fileName = "ShopItemTemplates")]
    public class ShopItemTemplatesStaticData : BaseStaticData<ShopItemTemplateData>
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