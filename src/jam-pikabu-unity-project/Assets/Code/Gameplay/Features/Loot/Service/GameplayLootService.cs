using System;
using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Service;

namespace Code.Gameplay.Features.Loot.Service
{
    public class GameplayLootService : IGameplayLootService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;
        private readonly ILootFactory _lootFactory;
        public event Action OnLootUpdate;

        private readonly List<LootTypeId> _collectedLootItems = new();

        private readonly List<LootSetup> _availableLoot = new();

        public bool LootIsBusy { get; private set; }
        public IReadOnlyList<LootTypeId> CollectedLootItems => _collectedLootItems;
        public IReadOnlyList<LootSetup> AvailableLoot => _availableLoot;

        public GameplayLootService(IStaticDataService staticDataService, IDaysService daysService,
            ILootFactory lootFactory)
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
            _lootFactory = lootFactory;
        }

        public void CreateLootSpawner()
        {
            InitLootBufferInternal();
            _lootFactory.CreateLootSpawner();
        }

        public void AddCollectedLoot(LootTypeId lootType)
        {
            _collectedLootItems.Add(lootType);
            NotifyLootUpdated();
        }

        public void SetLootIsConsumingState(bool state)
        {
            LootIsBusy = state;
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
            var staticData = _staticDataService.GetStaticData<LootSettingsStaticData>();
            var currentDay = _daysService.GetDayData();

            _availableLoot.Clear();
            foreach (LootTypeId lootTypeId in currentDay.AvailableIngredients)
            {
                _availableLoot.Add(staticData.GetConfig(lootTypeId));
            }
        }

        private void NotifyLootUpdated()
        {
            OnLootUpdate?.Invoke();
        }
    }
}