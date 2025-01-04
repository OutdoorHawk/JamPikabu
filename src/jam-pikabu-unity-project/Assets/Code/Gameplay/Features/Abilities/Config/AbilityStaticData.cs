using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(AbilityStaticData), fileName = "Ability")]
    public class AbilityStaticData : BaseStaticData<AbilityData>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();
            AddIndex(data => (int)data.Type);
        }

        public AbilityData GetDataByType(AbilityTypeId typeId)
        {
            return GetByKey((int)typeId);
        }
    }
}