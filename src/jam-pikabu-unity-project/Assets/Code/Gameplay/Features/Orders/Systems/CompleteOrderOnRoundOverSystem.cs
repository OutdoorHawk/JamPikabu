using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class CompleteOrderOnRoundOverSystem : IExecuteSystem
    {
        private readonly IWindowService _windowService;
        private readonly IUIFactory _uiFactory;
        private readonly IGroup<GameEntity> _orders;
        private readonly IGroup<GameEntity> _roundStateControllers;

        public CompleteOrderOnRoundOverSystem(GameContext context, IWindowService windowService, IUIFactory uiFactory)
        {
            _windowService = windowService;
            _uiFactory = uiFactory;
            _roundStateControllers = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController, 
                    GameMatcher.RoundOver));
            
            _orders = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Order,
                    GameMatcher.OrderData
                ));
        }

        public void Execute()
        {
            foreach (var _ in _roundStateControllers)
            foreach (var entity in _orders)
            {
                OrderSetup orderDataSetup = entity.OrderData.Setup;
                
                CreateGameEntity
                    .Empty()
                    .With(x => x.isAddCurrencyRequest = true)
                    .AddGold(orderDataSetup.Reward.Amount)
                    .AddWithdraw(orderDataSetup.Reward.Amount)
                    ;
                
                entity.isComplete = true;
                
                _windowService.Close(WindowTypeId.OrderWindow);
                _uiFactory.SetRaycastAvailable(false);
            }
        }
    }
}