using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.Orders.Windows;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class PlayOrderWindowOnLootConsumedVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly IWindowService _windowService;
        private readonly IOrdersService _ordersService;
        private readonly IUIFactory _uiFactory;
        private readonly ILootService _lootService;

        private readonly List<UniTask> _tasksBuffer = new();

        public PlayOrderWindowOnLootConsumedVisualsSystem(GameContext context, IWindowService windowService,
            IOrdersService ordersService, IUIFactory uiFactory, ILootService lootService) : base(context)
        {
            _windowService = windowService;
            _ordersService = ordersService;
            _uiFactory = uiFactory;
            _lootService = lootService;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher
                .AllOf(GameMatcher.LootEffectsApplier).Removed());
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var _ in entities)
            {
                PlayOrderCompleteAnimation().Forget();
            }
        }

        private async UniTask PlayOrderCompleteAnimation()
        {
            if (_lootService.CollectedLootItems.Count != 0)
            {
                var orderWindow = await _windowService.OpenWindow<OrderWindow>(WindowTypeId.OrderWindow);

                await orderWindow.PlayOrderComplete();

                _uiFactory.SetRaycastAvailable(true);

                foreach (var button in orderWindow.CloseButtons)
                    _tasksBuffer.Add(button.OnClickAsync());

                _tasksBuffer.Add(orderWindow.ExitButton.OnClickAsync());

                await UniTask.WhenAny(_tasksBuffer);

                _tasksBuffer.Clear();
            }
            
            _uiFactory.SetRaycastAvailable(true);
            
            CreateGameEntity
                .Empty()
                .With(x => x.isNextOrderRequest = true)
                ;
        }
    }
}