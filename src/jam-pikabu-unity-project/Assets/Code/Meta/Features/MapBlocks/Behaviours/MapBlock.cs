using System.Collections.Generic;
using System.Linq;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Behaviours;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.Features.MainMenu.Behaviours;
using Code.Meta.Features.MainMenu.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Meta.Features.MapBlocks.Behaviours
{
    public class MapBlock : MonoBehaviour
    {
        public RectTransform LevelsParent;
        public TMP_Text StarsEarned;
        public IngredientUnlockBehaviour UnlockableIngredient;

        private IDaysService _daysService;
        private ILootCollectionService _lootCollectionService;
        private IMapMenuService _mapMenuService;

        public List<LevelButton> LevelButtons { get; private set; } = new();

        [Inject]
        private void Construct(IDaysService daysService, 
            ILootCollectionService lootCollectionService, 
            IMapMenuService mapMenuService)
        {
            _mapMenuService = mapMenuService;
            _lootCollectionService = lootCollectionService;
            _daysService = daysService;
        }

        private void Awake()
        {
            LevelButtons.RefreshList(LevelsParent.GetComponentsInChildren<LevelButton>());
            _lootCollectionService.OnFreeUpgradeTimeEnd += InitUnlockableIngredient;
            _lootCollectionService.OnUpgraded += InitUnlockableIngredient;
        }

        public void Init()
        {
            InitStarsAmount();
            InitUnlockableIngredient();
        }

        private void OnDestroy()
        {
            _lootCollectionService.OnFreeUpgradeTimeEnd -= InitUnlockableIngredient;
            _lootCollectionService.OnUpgraded -= InitUnlockableIngredient;
        }

        private void InitStarsAmount()
        {
            int earnedStars = LevelButtons.Sum(levelButton => _daysService.GetStarsEarnedForDay(levelButton.DayId));
            int maxStars = LevelButtons.Sum(levelButton => _daysService.GetDayStarData(levelButton.DayId).Stars.Count);
            StarsEarned.text = $"{earnedStars}/{maxStars}";
        }

        private void InitUnlockableIngredient()
        {
            foreach (LevelButton levelButton in LevelButtons)
            {
                DayData dayData = _daysService.GetDayData(levelButton.DayId);

                MapBlockData mapBlockData = _mapMenuService.GetMapBlockData(dayData.Id);
                LootTypeId unlocksIngredient = mapBlockData.UnlocksIngredient;

                if (mapBlockData.UnlocksIngredient == LootTypeId.None)
                    continue;
                
                if (CheckPreviousDayIsCompleted() == false)
                {
                    SetIngredientLockedState(unlocksIngredient);
                    break;
                }

                if (CheckIngredientAlreadyUnlocked(unlocksIngredient))
                {
                    SetUpgradeState(unlocksIngredient);
                    break;
                }

                SetReadyToUnlockState(unlocksIngredient);
            }
        }

        private bool CheckIngredientAlreadyUnlocked(LootTypeId unlocksIngredient)
        {
            return _lootCollectionService.LootProgression.TryGetValue(unlocksIngredient, out _);
        }

        private bool CheckPreviousDayIsCompleted()
        {
            int lowestDay = PickLowestDayInBlock();
            int previousDayCompleted = lowestDay - 1;

            if (_daysService.TryGetDayProgress(previousDayCompleted, out _) == false)
                return false;

            return true;
        }

        private int PickLowestDayInBlock()
        {
            return LevelButtons[0].DayId;
        }

        private void SetUpgradeState(LootTypeId unlocksIngredient)
        {
            UnlockableIngredient.InitFreeUpgradeState(unlocksIngredient);
        }

        private void SetIngredientLockedState(LootTypeId unlocksIngredient)
        {
            UnlockableIngredient.InitLocked(unlocksIngredient);
        }

        private void SetReadyToUnlockState(LootTypeId unlocksIngredient)
        {
            UnlockableIngredient.InitReadyToUnlock(unlocksIngredient);
        }
    }
}