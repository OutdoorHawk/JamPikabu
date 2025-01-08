using System;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Distraction.Config;
using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Features.Distraction.Factory
{
    public class DistractionObjectsFactory : IDistractionObjectsFactory
    {
        private readonly IStaticDataService _staticData;

        public DistractionObjectsFactory(IStaticDataService staticData)
        {
            _staticData = staticData;
        }

        public void CreateDistractionObject(DistractionObjectTypeId typeId, Transform at)
        {
            DistractionObjectData data = _staticData.Get<DistractionObjectsStaticData>().GetDataByTypeId(typeId);

            var entity = CreateGameEntity
                    .Empty()
                    .With(x => x.isDistractionObject = true)
                    .AddDistractionObjectTypeId(typeId)
                    .AddViewPrefab(data.ViewPrefab)
                    .AddStartWorldPosition(at.position)
                    .AddTargetParent(at)
                ;

            switch (typeId)
            {
                case DistractionObjectTypeId.None:
                    break;
                case DistractionObjectTypeId.Bee:
                    CreateBee(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeId), typeId, null);
            }
        }

        private void CreateBee(GameEntity entity)
        {
            entity
                .With(x => x.isBee = true)
                ;
        }
    }
}