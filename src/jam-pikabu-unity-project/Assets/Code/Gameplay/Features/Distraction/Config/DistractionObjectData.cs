using System;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.View;

namespace Code.Gameplay.Features.Distraction.Config
{
    [Serializable]
    public class DistractionObjectData : BaseData
    {
        public DistractionObjectTypeId TypeId;
        public EntityView ViewPrefab;
    }
}