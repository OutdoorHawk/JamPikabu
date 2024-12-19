using System.Collections.Generic;
using System.Linq;
using Code.Common.Extensions;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.MainMenu.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class MapContainer : MonoBehaviour
    {
        public ScrollRect MainScroll;
        public MapContainerMover Mover;

        public List<MapBlock> MapBlocks { get; private set; } = new();

        private readonly List<LevelButton> _levelButtons = new();
        private readonly Dictionary<int, LevelButton> _buttonByDayIds = new();

        private IStaticDataService _staticDataService;
        private IMapMenuService _mapMenuService;
        private IDaysService _daysService;

        private DaysStaticData DaysStaticData => _staticDataService.GetStaticData<DaysStaticData>();

        [Inject]
        private void Construct
        (
            IStaticDataService staticDataService,
            IMapMenuService mapMenuService,
            IDaysService daysService
        )
        {
            _mapMenuService = mapMenuService;
            _staticDataService = staticDataService;
            _daysService = daysService;
        }

        private void Awake()
        {
            MapBlocks.RefreshList(MainScroll.content.GetComponentsInChildren<MapBlock>(true));
            
            foreach (MapBlock mapBlock in MapBlocks) 
                mapBlock.EnableElement();
        }

        public void Init()
        {
            InitLevels();
            SelectLastLevel();
            Refresh();
        }

        public void SubscribeUpdates()
        {
            _mapMenuService.OnSelectionChanged += Refresh;
        }

        public void Unsubscribe()
        {
            _mapMenuService.OnSelectionChanged -= Refresh;
        }

        private void InitLevels()
        {
            InitLevelList();
            BindLevelToDayId();
            LockAllDaysOutsideOfConfig();
            LockLevelsByProgress();
            LockLevelsByStars();
        }

        private void SelectLastLevel()
        {
            List<DayProgressData> dayProgressData = _daysService.GetDaysProgress();
            
            if (dayProgressData.Count == 0)
            {
                _buttonByDayIds[1].SelectLevel();
                return;
            }

            DayProgressData progressData = dayProgressData.Last();
            int dayToSelect = progressData.DayId + 1;
            if (dayToSelect >= _buttonByDayIds.Count)
                _buttonByDayIds.Last().Value.SelectLevel();
            else
                _buttonByDayIds[dayToSelect].SelectLevel();
        }

        private void InitLevelList()
        {
            _levelButtons.Clear();
            foreach (MapBlock mapBlock in MapBlocks)
            foreach (LevelButton levelButton in mapBlock.LevelButtons)
            {
                _levelButtons.Add(levelButton);
            }
        }

        private void BindLevelToDayId()
        {
            _buttonByDayIds.Clear();
            for (int i = 0; i < DaysStaticData.Configs.Count; i++)
            {
                DayData dayData = DaysStaticData.Configs[i];

                if (i >= _levelButtons.Count)
                    break;

                _levelButtons[i].InitLevel(dayData);
                _buttonByDayIds[dayData.Id] = _levelButtons[i];
            }
        }

        private void LockAllDaysOutsideOfConfig()
        {
            for (int i = DaysStaticData.Configs.Count; i < _levelButtons.Count; i++)
            {
                _levelButtons[i].InitInactive();
            }
        }

        private void LockLevelsByProgress()
        {
            List<DayProgressData> daysProgress = _daysService.GetDaysProgress();

            for (int i = 0; i < _levelButtons.Count; i++)
            {
                LevelButton levelButton = _levelButtons[i];

                if (daysProgress.Exists(x => x.DayId == levelButton.DayId))
                    continue;

                if (i + 1 >= _levelButtons.Count)
                    break;

                LevelButton nextLevel = _levelButtons[i + 1];
                nextLevel.SetLevelLocked();
            }
        }

        private void LockLevelsByStars()
        {
            for (int i = 0; i < _levelButtons.Count; i++)
            {
                LevelButton levelButton = _levelButtons[i];
                DayData dayData = _daysService.GetDayData(levelButton.DayId);

                if (_daysService.TryGetDayProgress(levelButton.DayId, out DayProgressData progress) == false)
                    continue;

                if (progress.StarsEarned < dayData.StarsNeedToUnlock)
                    levelButton.SetLevelLocked();
            }
        }

        private void Refresh()
        {
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
    }
}