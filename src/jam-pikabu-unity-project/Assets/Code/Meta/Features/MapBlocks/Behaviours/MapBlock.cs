﻿using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Localization;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.Features.MainMenu.Behaviours;
using Code.Meta.Features.MainMenu.Service;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MapBlocks.Behaviours
{
    public class MapBlock : MonoBehaviour, ILocalizationHandler
    {
        public RectTransform LevelsParent;
        public UnlockableIngredient UnlockableIngredient;
        public AvailableIngredientsView AvailableIngredients;
        public GameObject LockedContent;
        public TMP_Text LockedByStarsText;
        public TMP_Text LockedByLevelsText;
        public Image Background;
        public Image DecorForeground;
        public Image LockedBackground;
        public Image LockedBackground2;

        private IDaysService _daysService;
        private ILootCollectionService _lootCollectionService;
        private IMapMenuService _mapMenuService;
        private MapBlockData _mapBlockData;
        private ILocalizationService _localizationService;
        private IStaticDataService _staticData;

        public List<LevelButton> LevelButtons { get; private set; } = new();

        public MapBlockData BlockData => _mapBlockData;

        [Inject]
        private void Construct
        (
            IDaysService daysService,
            ILootCollectionService lootCollectionService,
            IMapMenuService mapMenuService,
            ILocalizationService localizationService,
            IStaticDataService staticData
        )
        {
            _staticData = staticData;
            _localizationService = localizationService;
            _mapMenuService = mapMenuService;
            _lootCollectionService = lootCollectionService;
            _daysService = daysService;
        }

        private void Start()
        {
            _localizationService.RegisterHandler(this);
        }

        private void OnDestroy()
        {
            _localizationService.UnregisterHandler(this);
        }

        public void OnLanguageChanged(Locale locale)
        {
            UpdateLockedStarsText();
        }

        public void InitData(MapBlockData mapBlockData)
        {
            _mapBlockData = mapBlockData;
            var staticData = _staticData.Get<MapBlocksStaticData>();

            if (staticData.EnableBackgroundsRandom)
            {
                Sprite backgroundSprite = staticData.MapBlockBackgrounds.GetCurrent();
                Background.sprite = backgroundSprite;
            }

            LockedBackground.sprite = Background.sprite;
            LockedBackground2.sprite = Background.sprite;
            DecorForeground.sprite = Background.sprite;
            LevelButtons.RefreshList(LevelsParent.GetComponentsInChildren<LevelButton>());
            UnlockableIngredient.Initialize(mapBlockData, Background.sprite);
            AvailableIngredients.Init(mapBlockData);
            InitLockedState();
        }

        private void InitLockedState()
        {
            LockedContent.DisableElement();
            LockedBackground2.DisableElement();
            LockedByLevelsText.DisableElement();
            LockedByStarsText.DisableElement();

            if (_mapMenuService.ChekMapBlockIsAvailableByStars(_mapBlockData) == false)
            {
                if (UnlockableIngredient.gameObject.activeSelf == false) 
                    LockedContent.EnableElement();
                else
                    LockedBackground2.EnableElement();
            
                LockedByStarsText.EnableElement();
                UpdateLockedStarsText();
                return;
            }

            if (_mapMenuService.CheckMapBlockIsAvailableByLevels(_mapBlockData) == false)
            {
                if (UnlockableIngredient.gameObject.activeSelf == false) 
                    LockedContent.EnableElement();
                else
                    LockedBackground2.EnableElement();
                
                LockedByLevelsText.EnableElement();
            }
        }

        private void UpdateLockedStarsText()
        {
            if (_mapBlockData == null)
                return;

            int diff = _mapBlockData.StarsNeedToUnlock - _daysService.GetAllEarnedStars();
            LockedByStarsText.text = _localizationService["MAIN MENU/STARS_NEED_TO_UNLOCK", diff.ToString()];
        }
    }
}