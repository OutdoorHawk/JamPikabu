using System.Collections.Generic;
using Code.Gameplay.Features.Consumables.Config;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.LootSpawning.Factory;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Consumables;
using Entitas;

namespace Code.Gameplay.Features.Consumables.Systems
{
    public class ActivateWoodConsumableSystem : IExecuteSystem
    {
        private readonly ILootSpawnerFactory _lootSpawnerFactory;
        private readonly IStaticDataService _staticDataService;
        
        private readonly IGroup<GameEntity> _requests;

        public ActivateWoodConsumableSystem(GameContext context, ILootSpawnerFactory lootSpawnerFactory, IStaticDataService staticDataService)
        {
            _lootSpawnerFactory = lootSpawnerFactory;
            _staticDataService = staticDataService;
            
            _requests = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.ActivateConsumableRequest,
                    GameMatcher.Wood,
                    GameMatcher.Processed
                ));
        }

        public void Execute()
        {
            foreach (var _ in _requests)
            {
                ConsumablesData data = _staticDataService
                    .Get<ConsumablesStaticData>()
                    .GetConsumableData(ConsumableTypeId.Wood);
                
                var lootToSpawn = new List<LootTypeId>();

                for (int i = 0; i < data.Value; i++)
                    lootToSpawn.Add(LootTypeId.WoodChip);

                _lootSpawnerFactory.CreateOneTimeSpawner(lootToSpawn);
            }
        }
    }
}