using System.Collections.Generic;
using System.Linq;
using Code.Common.Extensions;
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
        }

        public void Init()
        {
            InitStarsAmount();
            UnlockableIngredient.Initialize(LevelButtons);
        }

        private void OnDestroy()
        {
        }

        private void InitStarsAmount()
        {
            int earnedStars = LevelButtons.Sum(levelButton => _daysService.GetStarsEarnedForDay(levelButton.DayId));
            int maxStars = LevelButtons.Sum(levelButton => _daysService.GetDayStarData(levelButton.DayId).Stars.Count);
            StarsEarned.text = $"{earnedStars}/{maxStars}";
        }
    }
}