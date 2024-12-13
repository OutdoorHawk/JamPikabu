using System;
using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;

namespace Code.Gameplay.Features.Loot.Service
{
    public class LootService : ILootService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IRoundStateService _roundStateService;
        private readonly ILootFactory _lootFactory;
        public event Action OnLootUpdate;
        public event Action<LootTypeId> OnLootItemAdded;

        private readonly List<LootTypeId> _collectedLootItems = new();

        private readonly List<LootSetup> _availableLoot = new();

        public IReadOnlyList<LootTypeId> CollectedLootItems => _collectedLootItems;
        public IReadOnlyList<LootSetup> AvailableLoot => _availableLoot;

        public LootService(IStaticDataService staticDataService, IRoundStateService roundStateService,
            ILootFactory lootFactory)
        {
            _staticDataService = staticDataService;
            _roundStateService = roundStateService;
            _lootFactory = lootFactory;
        }

        public void CreateLootSpawner()
        {
            InitLootBufferInternal();
            _lootFactory.CreateLootSpawner();
        }

        public void CreateNewCollectedLootItem(LootTypeId lootType)
        {
            _collectedLootItems.Add(lootType);
            NotifyLootItemAdded(lootType);
            NotifyLootUpdated();
        }

        public void ClearCollectedLoot()
        {
            _collectedLootItems.Clear();
            NotifyLootUpdated();
        }

        public void CreateLootConsumer()
        {
            CreateGameEntity
                .Empty()
                .With(x => x.isLootEffectsApplier = true)
                .With(x => x.isAvailable = true)
                ;
        }

        private void InitLootBufferInternal()
        {
            List<LootSetup> configs = _staticDataService.GetStaticData<LootStaticData>().Configs;
            int currentDay = _roundStateService.CurrentDay;

            _availableLoot.Clear();
            foreach (var config in configs)
            {
                if (CheckMinDayToUnlock(config, currentDay))
                    continue;

                if (CheckMaxDayToUnlock(config, currentDay))
                    continue;

                _availableLoot.Add(config);
            }
        }

        private static bool CheckMinDayToUnlock(LootSetup data, int currentDay)
        {
            return data.MinDayToUnlock > 0 && currentDay < data.MinDayToUnlock;
        }

        private static bool CheckMaxDayToUnlock(LootSetup data, int currentDay)
        {
            return data.MaxDayToUnlock > 0 && currentDay > data.MaxDayToUnlock;
        }

        private void NotifyLootItemAdded(LootTypeId lootType)
        {
            OnLootItemAdded?.Invoke(lootType);
        }


        private void NotifyLootUpdated()
        {
            OnLootUpdate?.Invoke();
        }
    }
}