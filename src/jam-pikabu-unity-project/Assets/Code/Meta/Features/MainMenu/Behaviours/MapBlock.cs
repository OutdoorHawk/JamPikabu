using System.Collections.Generic;
using System.Linq;
using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Loot;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Service;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class MapBlock : MonoBehaviour
    {
        public RectTransform LevelsParent;
        public TMP_Text StarsEarned;
        public UnlockableIngredient UnlockableIngredient;

        private IDaysService _daysService;
        private ILootCollectionService _lootCollectionService;
        private List<LevelButton> _buffer = new();

        public List<LevelButton> LevelButtons { get; private set; } = new();

        [Inject]
        private void Construct(IDaysService daysService, ILootCollectionService lootCollectionService)
        {
            _lootCollectionService = lootCollectionService;
            _daysService = daysService;
        }

        private void Awake()
        {
            LevelButtons.RefreshList(LevelsParent.GetComponentsInChildren<LevelButton>());
        }

        private void Start()
        {
            InitStarsAmount();
            InitUnlockableIngredient();
        }

        private void InitStarsAmount()
        {
            int earnedStars = LevelButtons.Sum(levelButton => _daysService.GetStarsEarnedForDay(levelButton.DayId));
            int maxStars = LevelButtons.Sum(levelButton => _daysService.GetDayData(levelButton.DayId).Stars.Count);
            StarsEarned.text = $"{earnedStars}/{maxStars}";
        }

        private void InitUnlockableIngredient()
        {
            foreach (LevelButton levelButton in LevelButtons)
            {
                DayData dayData = _daysService.GetDayData(levelButton.DayId);

                if (dayData.UnlocksIngredient == LootTypeId.Unknown)
                    continue;

                if (CheckIngredientAlreadyUnlocked(dayData))
                    continue;

                if (CheckPreviousDayIsCompleted() == false)
                {
                    SetIngredientLockedState(dayData.UnlocksIngredient);
                    continue;
                }

                SetReadyToUnlockState(dayData.UnlocksIngredient);
            }
        }

        private bool CheckIngredientAlreadyUnlocked(DayData dayData)
        {
            return _lootCollectionService.LootProgression.TryGetValue(dayData.UnlocksIngredient, out _);
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
            _buffer.Clear();
            _buffer.AddRange(LevelButtons);
            _buffer.Sort((x, y) => x.DayId.CompareTo(y.DayId));
            return _buffer[0].DayId;
        }

        private void SetIngredientLockedState(LootTypeId unlocksIngredient)
        {
            UnlockableIngredient.InitLocked(unlocksIngredient);
        }

        private void SetReadyToUnlockState(LootTypeId unlocksIngredient)
        {
            UnlockableIngredient.InitReadyToCollect(unlocksIngredient);
        }
    }
}