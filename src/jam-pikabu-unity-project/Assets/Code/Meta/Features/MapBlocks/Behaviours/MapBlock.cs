using System.Collections.Generic;
using System.Linq;
using Code.Common.Extensions;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Service;
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
        public UnlockableIngredient UnlockableIngredient;

        private IDaysService _daysService;
        private ILootCollectionService _lootCollectionService;
        private IMapMenuService _mapMenuService;
        private MapBlockData _mapBlockData;

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

        public void InitData(MapBlockData mapBlockData)
        {
            _mapBlockData = mapBlockData;
            LevelButtons.RefreshList(LevelsParent.GetComponentsInChildren<LevelButton>());
            InitStarsAmount();
            UnlockableIngredient.Initialize(mapBlockData);
        }

        private void InitStarsAmount()
        {
            int earnedStars = LevelButtons.Sum(levelButton => _daysService.GetStarsEarnedForDay(levelButton.DayId));
            int maxStars = LevelButtons.Sum(levelButton => _daysService.GetDayStarData(levelButton.DayId).Stars.Count);
            StarsEarned.text = $"{earnedStars}/{maxStars}";
        }
    }
}