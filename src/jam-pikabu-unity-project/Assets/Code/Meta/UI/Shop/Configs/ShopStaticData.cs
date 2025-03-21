using System.Collections.Generic;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.ABTesting;
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
            InitAbTest();
        }

        public List<ShopItemData> GetByItemKind(ShopItemKind kind)
        {
            return GetNonUniqueByKey((int)kind);
        }

        public ShopItemData GetByConsumableType(ConsumableTypeId consumableType)
        {
            return GetByKey((int)consumableType);
        }

        private void InitAbTest()
        {
            if (_abTestService.GetExperimentValue(ExperimentTagTypeId.TIMER_REPLACE) is not ExperimentValueTypeId.replace_timer_with_attempts)
            {
                GetByConsumableType(ConsumableTypeId.Wood).DescriptionLocale.TableEntryReference = "S_WOOD_CONSUMABLE_DESCRIPTION";
                return;
            }

            GetByConsumableType(ConsumableTypeId.Wood).DescriptionLocale.TableEntryReference = "S_WOOD_CONSUMABLE_DESCRIPTION_B";
        }
    }
}