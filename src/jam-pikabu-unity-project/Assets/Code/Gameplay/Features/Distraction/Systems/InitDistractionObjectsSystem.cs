using Code.Gameplay.Features.Distraction.Factory;
using Code.Infrastructure.SceneContext;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Distraction.Systems
{
    public class InitDistractionObjectsSystem : IInitializeSystem
    {
        private readonly IDistractionObjectsFactory _distractionObjectsFactory;
        private readonly IDaysService _daysService;
        private readonly ISceneContextProvider _sceneContext;

        public InitDistractionObjectsSystem(IDistractionObjectsFactory distractionObjectsFactory, IDaysService daysService, ISceneContextProvider sceneContext)
        {
            _distractionObjectsFactory = distractionObjectsFactory;
            _daysService = daysService;
            _sceneContext = sceneContext;
        }

        public void Initialize()
        {
            int spawnIndex = 0;
            DayData dayData = _daysService.GetDayData();
            Transform[] lootSpawnPoints = _sceneContext.Context.LootSpawnPoints;

            foreach (DistractionObjectTypeId typeId in dayData.DistractionObjects)
            {
                if (spawnIndex >= lootSpawnPoints.Length) 
                    spawnIndex = 0;
                
                _distractionObjectsFactory.CreateDistractionObject(typeId, lootSpawnPoints[0].parent);
                spawnIndex++;
            }
        }
    }
}