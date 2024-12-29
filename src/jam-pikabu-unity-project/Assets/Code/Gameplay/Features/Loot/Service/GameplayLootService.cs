using System;
using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Orders;
using Code.Gameplay.StaticData;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Service;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;

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
            var staticData = _staticDataService.Get<LootSettingsStaticData>();
            var dayLootSettingsStaticData = _staticDataService.Get<MapBlocksStaticData>();
            var currentDay = _daysService.GetDayData();

            if (_daysService.BonusLevelType is BonusLevelType.GoldenCoins)
            {
                BonusLevelData bonusLevelData = _staticDataService.Get<BonusLevelStaticData>().Configs[0];
                foreach (LootTypeId typeId in bonusLevelData.AvailableIngredients)
                {
                    _availableLoot.Add(staticData.GetConfig(typeId));
                }
            }
            else
            {
                MapBlockData mapBlock = dayLootSettingsStaticData.GetMapBlockDataByDayId(currentDay.Id);

                _availableLoot.Clear();
                foreach (LootTypeId lootTypeId in mapBlock.AvailableIngredients)
                {
                    _availableLoot.Add(staticData.GetConfig(lootTypeId));
                }
                foreach (LootTypeId lootTypeId in mapBlock.ExtraLoot)
                {
                    _availableLoot.Add(staticData.GetConfig(lootTypeId));
                }
            }

            FallbackRandom(staticData);
        }

        private void FallbackRandom(LootSettingsStaticData staticData)
        {
            if (_availableLoot.Count != 0)
                return;
            
            List<LootTypeId> lootTypes = new List<LootTypeId>();
            
            for (LootTypeId i = 0; i < LootTypeId.Count; i++)
                lootTypes.Add(i);

            lootTypes.ShuffleList();

            for (int i = 0; i < 5; i++)
            {
                _availableLoot.Add(staticData.GetConfig(lootTypes[i]));
            }
        }

        private void NotifyLootUpdated()
        {
            OnLootUpdate?.Invoke();
        }
    }
}