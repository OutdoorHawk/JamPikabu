using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Gameplay.Features.Distraction.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(DistractionObjectsStaticData), fileName = "DistractionObjects")]
    public class DistractionObjectsStaticData : BaseStaticData<DistractionObjectData>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();
            
            AddIndex(0, data => (int)data.TypeId);
        }

        public DistractionObjectData GetDataByTypeId(DistractionObjectTypeId typeId)
        {
            return GetByKey((int)typeId, 0);
        }
    }
}