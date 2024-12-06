using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.Orders.Windows;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class PlayOrderWindowVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly IWindowService _windowService;
        private readonly IOrdersService _ordersService;
        private readonly IUIFactory _uiFactory;

        public PlayOrderWindowVisualsSystem(GameContext context, IWindowService windowService, 
            IOrdersService ordersService, IUIFactory uiFactory) : base(context)
        {
            _windowService = windowService;
            _ordersService = ordersService;
            _uiFactory = uiFactory;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher
                .AllOf(GameMatcher.RoundStateController,
                    GameMatcher.RoundComplete).Added());
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
            var orderWindow = await _windowService.OpenWindow<OrderWindow>(WindowTypeId.OrderWindow);

            await orderWindow.PlayOrderComplete();
            
            _uiFactory.SetRaycastAvailable(true);

            await orderWindow.ExitButton.OnClickAsync(); // TODO: ВСЕ СПОСОБЫ ВЫХОДА

            var order = _ordersService.GetCurrentOrder();

            CreateGameEntity
                .Empty()
                .With(x => x.isAddCurrencyRequest = true)
                .AddGold(0)
                .AddWithdraw(-order.Setup.Reward.Amount)
                ;
            
            Debug.LogError($"remove withdraw {order.Setup.Reward.Amount}");
            
            _ordersService.GoToNextOrder();
        }
    }
}