using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Extensions;
using Code.Meta.Features.Days.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class MapBlock : MonoBehaviour
    {
        public RectTransform LevelsParent;
        public TMP_Text StarsEarned;
        
        private IDaysService _daysService;

        public List<LevelButton> LevelButtons { get; private set; } = new();

        [Inject]
        private void Construct(IDaysService daysService)
        {
            _daysService = daysService;
        }

        private void Awake()
        {
            LevelButtons.RefreshList(LevelsParent.GetComponentsInChildren<LevelButton>());
        }

        private void Start()
        {
            InitStarsAmount();
        }

        private void InitStarsAmount()
        {
            int earnedStars = LevelButtons.Sum(levelButton => _daysService.GetStarsEarnedForDay(levelButton.DayId));
            int maxStars = LevelButtons.Sum(levelButton => _daysService.GetDayData(levelButton.DayId).Stars.Count);
            StarsEarned.text = $"{earnedStars}/{maxStars}";
        }
    }
}