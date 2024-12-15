using System.Collections.Generic;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.MainMenu.Behaviours;
using Code.Meta.Features.MainMenu.Service;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MainMenu.Windows
{
    public class MainMenuWindow : BaseWindow
    {
        public MapContainer MapContainer;
        public Button PlayButton;

        private IGameStateMachine _gameStateMachine;
        private IMapMenuService _mapMenuService;
        private IStaticDataService _staticDataService;
        private IDaysService _daysService;

        [Inject]
        private void Construct
        (
            IGameStateMachine gameStateMachine,
            IMapMenuService mapMenuService,
            IStaticDataService staticDataService,
            IDaysService daysService
        )
        {
            _daysService = daysService;
            _staticDataService = staticDataService;
            _mapMenuService = mapMenuService;
            _gameStateMachine = gameStateMachine;
        }

        protected override void Initialize()
        {
            base.Initialize();
            MapContainer.Init();
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();
            PlayButton.onClick.AddListener(OnPlayClick);
            _mapMenuService.OnSelectionChanged += Refresh;
            MapContainer.SubscribeUpdates();
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            PlayButton.onClick.RemoveListener(OnPlayClick);
            _mapMenuService.OnSelectionChanged -= Refresh;
            MapContainer.Unsubscribe();
        }

        private void Refresh()
        {
            PlayButton.interactable = _mapMenuService.DayIsSelected;
        }

        private void OnPlayClick()
        {
            _daysService.SetActiveDay(_mapMenuService.SelectedDayId);
            var parameters = new LoadLevelPayloadParameters
            {
                LevelName = _mapMenuService.GetSelectedScene().ToString()
            };

            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(parameters);
        }
    }
}