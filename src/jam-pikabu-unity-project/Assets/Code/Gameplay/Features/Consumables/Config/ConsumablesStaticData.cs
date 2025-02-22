using Code.Gameplay.StaticData.Data;
using Code.Meta.Features.Consumables;
using UnityEngine;

namespace Code.Gameplay.Features.Consumables.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(ConsumablesStaticData), fileName = "Consumables")]
    public class ConsumablesStaticData : BaseStaticData<ConsumablesData>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();
            AddIndex(0, data => (int)data.TypeId);
        }
        
        public ConsumablesData GetConsumableData(ConsumableTypeId id)
        {
            return GetByKey(0, (int)id);
        }
    }
}