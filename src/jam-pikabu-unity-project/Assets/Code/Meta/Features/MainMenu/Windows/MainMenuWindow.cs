using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Infrastructure.Analytics;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.BonusLevel.Behaviours;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.Features.MainMenu.Behaviours;
using Code.Meta.Features.MainMenu.Service;
using Code.Meta.Features.MapBlocks.Behaviours;
using Cysharp.Threading.Tasks;
using GamePush;
using TMPro;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MainMenu.Windows
{
    public class MainMenuWindow : BaseWindow
    {
        public MapContainer MapContainer;
        public Button PlayButton;
        public Button ShopButton;
        public BonusLevelButton BonusLevelButton;
        public TMP_Text PlayerIdText;

        private IGameStateMachine _gameStateMachine;
        private IMapMenuService _mapMenuService;
        private IStaticDataService _staticDataService;
        private IDaysService _daysService;
        private ILootCollectionService _lootCollectionService;
        private IAnalyticsService _analyticsService;

        [Inject]
        private void Construct
        (
            IGameStateMachine gameStateMachine,
            IMapMenuService mapMenuService,
            IStaticDataService staticDataService,
            IDaysService daysService,
            ILootCollectionService lootCollectionService,
            IAnalyticsService analyticsService
        )
        {
            _analyticsService = analyticsService;
            _lootCollectionService = lootCollectionService;
            _daysService = daysService;
            _staticDataService = staticDataService;
            _mapMenuService = mapMenuService;
            _gameStateMachine = gameStateMachine;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitPlayerId();
            MapContainer.Init();
            Refresh();
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();
            PlayButton.onClick.AddListener(OnPlayClick);
            _mapMenuService.OnSelectionChanged += Refresh;
            _lootCollectionService.OnNewLootUnlocked += Refresh;
            MapContainer.SubscribeUpdates();
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            PlayButton.onClick.RemoveListener(OnPlayClick);
            _mapMenuService.OnSelectionChanged -= Refresh;
            _lootCollectionService.OnNewLootUnlocked -= Refresh;
            MapContainer.Unsubscribe();
        }

        private void Refresh()
        {
            RefreshPlayButtonAsync().Forget();
        }

        private async UniTaskVoid RefreshPlayButtonAsync()
        {
            if (PlayButton == null)
                return;
            
            await UniTask.Yield(destroyCancellationToken);

            foreach (MapBlock mapBlock in MapContainer.MapBlocks)
            foreach (LevelButton level in mapBlock.LevelButtons)
            {
                if (_mapMenuService.SelectedDayId != level.DayId)
                    continue;

                if (mapBlock.UnlockableIngredient.ReadyToUnlock == false)
                    continue;

                PlayButton.interactable = false;
                return;
            }

            PlayButton.interactable = _mapMenuService.DayIsSelected;
        }

        private void OnPlayClick()
        {
            DayData dayData = _daysService.GetDayData(_mapMenuService.SelectedDayId);
            _daysService.SetActiveDay(dayData);

            var parameters = new LoadLevelPayloadParameters
            {
                LevelName = _mapMenuService.GetSelectedScene().ToString()
            };

            _gameStateMachine.Enter<LoadLevelState, LoadLevelPayloadParameters>(parameters);
        }

        private void InitPlayerId()
        {
            PlayerIdText.text = $"ID: {GP_Player.GetID()}";
        }
    }
}