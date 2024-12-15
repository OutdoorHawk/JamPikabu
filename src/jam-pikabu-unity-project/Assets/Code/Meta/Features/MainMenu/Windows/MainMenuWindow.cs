using System.Collections.Generic;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Configs;
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

        private readonly List<LevelButton> _levelButtons = new();

        private DaysStaticData DaysStaticData => _staticDataService.GetStaticData<DaysStaticData>();

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine, IMapMenuService mapMenuService, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _mapMenuService = mapMenuService;
            _gameStateMachine = gameStateMachine;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitLevels();
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();
            PlayButton.onClick.AddListener(OnPlayClick);
            _mapMenuService.OnSelectionChanged += Refresh;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            PlayButton.onClick.RemoveListener(OnPlayClick);
            _mapMenuService.OnSelectionChanged -= Refresh;
        }

        private void InitLevels()
        {
            _levelButtons.Clear();

            foreach (MapBlock mapBlock in MapContainer.MapBlocks)
            foreach (LevelButton levelButton in mapBlock.LevelButtons)
            {
                _levelButtons.Add(levelButton);
            }

            for (int i = 0; i < DaysStaticData.Configs.Count; i++)
            {
                DayData dayData = DaysStaticData.Configs[i];

                if (i >= _levelButtons.Count)
                    break;

                _levelButtons[i].InitLevel(dayData);
            }

            for (int i = DaysStaticData.Configs.Count; i < _levelButtons.Count; i++)
            {
                _levelButtons[i].InitInactive();
            }
        }

        private void Refresh()
        {
            PlayButton.interactable = _mapMenuService.DayIsSelected;

            UpdateSelectionView();
        }

        private void UpdateSelectionView()
        {
            foreach (LevelButton levelButton in _levelButtons)
            {
                if (_mapMenuService.DayIsSelected == false)
                {
                    levelButton.SetDeselectedView();
                    continue;
                }

                if (levelButton.DayId != _mapMenuService.SelectedDayId)
                {
                    levelButton.SetDeselectedView();
                    continue;
                }

                levelButton.SetSelectedView();
            }
        }

        private void OnPlayClick()
        {
            var parameters = new LoadLevelPayloadParameters
            {
                LevelName = _mapMenuService.GetSelectedScene().ToString()
            };

            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(parameters);
        }
    }
}