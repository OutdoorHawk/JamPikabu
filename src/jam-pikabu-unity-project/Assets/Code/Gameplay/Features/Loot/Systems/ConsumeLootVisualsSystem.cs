using System.Collections.Generic;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class ConsumeLootVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly IWindowService _windowService;
        private readonly IGameplayLootService _gameplayLootService;
        private readonly IGroup<GameEntity> _consumedLoot;

        public ConsumeLootVisualsSystem(GameContext context, IWindowService windowService, IGameplayLootService gameplayLootService) : base(context)
        {
            _windowService = windowService;
            _gameplayLootService = gameplayLootService;
            _consumedLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Consumed,
                    GameMatcher.Rating
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.LootEffectsApplier,
                GameMatcher.Available).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isLootEffectsApplier && entity.isAvailable;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var applier in entities)
            {
                applier.isAvailable = false;
                AnimateAsync(applier).Forget();
            }
        }

        private async UniTaskVoid AnimateAsync(GameEntity applier)
        {
            _gameplayLootService.SetLootIsConsumingState(true);

            _windowService.TryGetWindow<PlayerHUDWindow>(out var hud);
            var lootContainer = hud.GetComponentInChildren<GameplayLootContainer>();
            var orderView = hud.OrderViewBehaviour;

            await lootContainer.AnimateFlyToVat(_consumedLoot);
            orderView.InitOrderFillProgress();

            foreach (var loot in _consumedLoot)
                loot.isDestructed = true;

            applier.isDestructed = true;
            _gameplayLootService.SetLootIsConsumingState(false);
        }
    }
}