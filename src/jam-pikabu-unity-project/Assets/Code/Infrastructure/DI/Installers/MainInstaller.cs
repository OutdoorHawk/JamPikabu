using Code.Common.Logger.Service;
using Code.Gameplay.Cameras.Provider;
using Code.Gameplay.Cheats.Cheats;
using Code.Gameplay.Cheats.Service;
using Code.Gameplay.Common.Collisions;
using Code.Gameplay.Common.EntityIndices;
using Code.Gameplay.Common.MousePosition;
using Code.Gameplay.Common.Physics;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Common.Time.Service;
using Code.Gameplay.Features.Abilities.Factory;
using Code.Gameplay.Features.Consumables.Factory;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.Customers.Service;
using Code.Gameplay.Features.Distraction.Factory;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.GameState.Factory;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.GrapplingHook.Factory;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Loot.UIFactory;
using Code.Gameplay.Features.LootSpawning.Factory;
using Code.Gameplay.Features.Orders.Factory;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.Result.Service;
using Code.Gameplay.Features.RoundState.Factory;
using Code.Gameplay.Features.TextNotification.Service;
using Code.Gameplay.Input.Service;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Tutorial.Service;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.ABTesting;
using Code.Infrastructure.Ads.Service;
using Code.Infrastructure.Analytics;
using Code.Infrastructure.AssetManagement.AssetDownload;
using Code.Infrastructure.AssetManagement.AssetProvider;
using Code.Infrastructure.Common.CoroutineRunner;
using Code.Infrastructure.Common.GameIdentifier;
using Code.Infrastructure.Integrations.GamePush;
using Code.Infrastructure.Integrations.Service;
using Code.Infrastructure.Localization;
using Code.Infrastructure.SceneContext;
using Code.Infrastructure.SceneLoading;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Code.Infrastructure.View.Factory;
using Code.Meta.Features.BonusLevel.Service;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.Features.Days.Factory;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Factory;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.Features.MainMenu.Service;
using Code.Meta.Features.MainMenu.UIFactory;
using Code.Meta.UI.HardCurrencyHolder.Service;
using Code.Meta.UI.PreviewItem.Service;
using Code.Meta.UI.Shop.Factory;
using Code.Meta.UI.Shop.Service;
using Code.Meta.UI.Shop.WindowService;
using Code.Progress.Provider;
using Code.Progress.SaveLoadService;
using Code.Progress.Writer;
using RSG;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.DI.Installers
{
    public class MainInstaller : MonoInstaller, ICoroutineRunner, IInitializable
    {
        [SerializeField] private SoundService _soundService;
        [SerializeField] private SceneLoader _sceneLoader;

        public void Initialize()
        {
            Promise.UnhandledException += LogPromiseException;
        }

        public override void InstallBindings()
        {
            InjectableInstaller.Install(Container);
            BindGameStateMachine();
            BindContexts();
            BindInfrastructureServices();
            BindGameplayServices();
            BindProgressServices();
            BindCommonServices();
            BindInputService();
            BindFactories();
            BindGameplayFactories();
            BindEntityIndices();
            BindUIServices();
            BindUIFactories();
            BindIntegrations();
#if UNITY_EDITOR || CHEAT
            Container.BindInterfacesAndSelfTo<CheatsService>().AsSingle().NonLazy();
            Container.BindInterfacesTo<SetTutorialStepCheat>().AsSingle();
#endif
        }

        private void LogPromiseException(object sender, ExceptionEventArgs e)
        {
            Debug.LogError(e.Exception);
        }

        private void BindGameStateMachine()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();

            Container.Bind<IStateFactory>().To<StateFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadProgressState>().AsSingle();
            Container.BindInterfacesAndSelfTo<EditorLoadSceneState>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadMapMenuState>().AsSingle();
            Container.BindInterfacesAndSelfTo<MapMenuState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameEnterState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameLoopState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameOverState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameWinState>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadLevelState>().AsSingle();
        }

        private void BindContexts()
        {
            Container.Bind<Contexts>().FromInstance(Contexts.sharedInstance).AsSingle();
            Container.Bind<GameContext>().FromInstance(Contexts.sharedInstance.game).AsSingle();
            Container.Bind<InputContext>().FromInstance(Contexts.sharedInstance.input).AsSingle();
            Container.Bind<MetaContext>().FromInstance(Contexts.sharedInstance.meta).AsSingle();
        }

        private void BindInfrastructureServices()
        {
            Container.BindInterfacesTo<MainInstaller>().FromInstance(this).AsSingle();
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.Bind<IIdentifierService>().To<GameIdentifierService>().AsSingle();
            Container.Bind<ISceneContextProvider>().To<SceneContextProvider>().AsSingle();
            Container.Bind<IGameStateHandlerService>().To<GameStateHandlerService>().AsSingle();
            Container.BindInterfacesTo<LocalizationService>().AsSingle().NonLazy();
            Container.BindInterfacesTo<TutorialService>().AsSingle();
            // Container.BindInterfacesTo<AssetDownloadService>().AsSingle();
            Container.BindInterfacesTo<LabeledAssetDownloadService>().AsSingle();
            Container.BindInterfacesTo<AssetDownloadReporter>().AsSingle();
        }

        private void BindGameplayServices()
        {
            Container.BindInterfacesTo<SoundService>().FromInstance(_soundService).AsSingle();
            Container.BindInterfacesTo<GameplayCurrencyService>().AsSingle();
            Container.BindInterfacesTo<GameOverService>().AsSingle();
            Container.BindInterfacesTo<DaysService>().AsSingle();
            Container.BindInterfacesTo<OrdersService>().AsSingle();
            Container.BindInterfacesTo<CustomersService>().AsSingle();
            Container.BindInterfacesTo<GameStateService>().AsSingle();
            Container.BindInterfacesTo<MapMenuService>().AsSingle();
            Container.BindInterfacesTo<BonusLevelService>().AsSingle();
            Container.BindInterfacesTo<NotificationTextService>().AsSingle();
            Container.BindInterfacesTo<PreviewWindowService>().AsSingle();
            Container.BindInterfacesTo<LocalizedTimeService>().AsSingle();
            Container.BindInterfacesTo<ResultWindowService>().AsSingle();
            Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
            Container.Bind<IMousePositionService>().To<MousePositionService>().AsSingle();
            Container.Bind<ICameraProvider>().To<CameraProvider>().AsSingle();
        }

        private void BindProgressServices()
        {
            Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle();
            Container.Bind<IProgressProvider>().To<ProgressProvider>().AsSingle();
        }

        private void BindCommonServices()
        {
            Container.Bind<ICollisionRegistry>().To<CollisionRegistry>().AsSingle();
            Container.Bind<IPhysics2DService>().To<Physics2DService>().AsSingle();
            Container.Bind<ITimeService>().To<UnityTimeService>().AsSingle();
            Container.Bind<ISceneLoader>().FromInstance(_sceneLoader).AsSingle();

            Container.BindInterfacesTo<DefaultLogger>().AsSingle();
        }

        private void BindInputService()
        {
            Container.Bind<IInputService>().To<InputService>().AsSingle();
        }

        private void BindFactories()
        {
            Container.Bind<ISystemFactory>().To<SystemFactory>().AsSingle();
            Container.Bind<IShopItemFactory>().To<ShopItemFactory>().AsSingle();
            Container.Bind<IGrapplingHookFactory>().To<GrapplingHookFactory>().AsSingle();
            Container.Bind<ICurrencyFactory>().To<CurrencyFactory>().AsSingle();
            Container.Bind<ILootFactory>().To<LootFactory>().AsSingle();
            Container.Bind<ILootSpawnerFactory>().To<LootSpawnerFactory>().AsSingle();
            Container.BindInterfacesTo<LootItemUIFactory>().AsSingle();
            Container.BindInterfacesTo<RoundStateFactory>().AsSingle();
            Container.BindInterfacesTo<OrdersFactory>().AsSingle();
            Container.BindInterfacesTo<GameStateFactory>().AsSingle();
            Container.BindInterfacesTo<DaysFactory>().AsSingle();
            Container.BindInterfacesTo<LootCollectionFactory>().AsSingle();
            Container.BindInterfacesTo<MapMenuFactory>().AsSingle();
            Container.BindInterfacesTo<AbilityFactory>().AsSingle();
            Container.BindInterfacesTo<DistractionObjectsFactory>().AsSingle();
            Container.BindInterfacesTo<ConsumablesFactory>().AsSingle();
        }

        private void BindGameplayFactories()
        {
            Container.Bind<IEntityViewFactory>().To<EntityViewFactory>().AsSingle();
        }

        private void BindEntityIndices()
        {
            Container.BindInterfacesAndSelfTo<GameEntityIndices>().AsSingle();
        }

        private void BindUIServices()
        {
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            Container.BindInterfacesTo<StorageUIService>().AsSingle();
            Container.Bind<IShopUIService>().To<ShopUIService>().AsSingle();
            Container.BindInterfacesTo<GameplayLootService>().AsSingle();
            Container.BindInterfacesTo<ShopWindowService>().AsSingle();
            Container.BindInterfacesTo<LootCollectionService>().AsSingle();
            Container.BindInterfacesTo<ConsumablesUIService>().AsSingle();
        }

        private void BindUIFactories()
        {
            Container.Bind<IUIFactory>().To<UIFactory>().AsSingle();
        }

        private void BindIntegrations()
        {
            Container.BindInterfacesTo<IntegrationsService>().AsSingle();
            
#if GAME_PUSH
            Container.BindInterfacesTo<AppMetricaAnalyticsService>().AsSingle();
            Container.BindInterfacesTo<GamePushIntegration>().AsSingle();
            Container.BindInterfacesTo<GamePushAdsService>().AsSingle();
            Container.BindInterfacesTo<GamePushProgressReadWrite>().AsSingle();
            Container.BindInterfacesTo<GamePushExperimentService>().AsSingle();
#else
            Container.Bind<IProgressReadWrite>().To<DefaultFileProgressReadWrite>().AsSingle();
            Container.BindInterfacesTo<FakeAdsService>().AsSingle();
            Container.BindInterfacesTo<FakeAnalyticsService>().AsSingle();
#endif
        }
    }
}