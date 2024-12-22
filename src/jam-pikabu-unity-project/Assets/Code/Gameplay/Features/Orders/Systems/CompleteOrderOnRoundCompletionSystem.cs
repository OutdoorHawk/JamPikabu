using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class CompleteOrderOnRoundCompletionSystem : IExecuteSystem
    {
        private readonly IWindowService _windowService;
        private readonly IOrdersService _ordersService;
        private readonly IGroup<GameEntity> _orders;
        private readonly IGroup<GameEntity> _busyLoot;
        private readonly List<GameEntity> _buffer = new(1);
        private readonly IGroup<GameEntity> _gameState;
        private readonly IGroup<GameEntity> _collectedLoot;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly IGameplayLootService _gameplayLootService;

        public CompleteOrderOnRoundCompletionSystem(GameContext context, IWindowService windowService
            , IOrdersService ordersService, ICurrencyFactory currencyFactory, IGameplayLootService gameplayLootService)
        {
            _currencyFactory = currencyFactory;
            _gameplayLootService = gameplayLootService;
            _windowService = windowService;
            _ordersService = ordersService;

            _gameState = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.GameState,
                    GameMatcher.RoundCompletion));
            
            _collectedLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Collected));

            _orders = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Order,
                    GameMatcher.OrderData
                ).NoneOf(
                    GameMatcher.Complete));
        }

        public void Execute()
        {
            foreach (var game in _gameState)
            foreach (var order in _orders.GetEntities(_buffer))
            {
                if (_collectedLoot.count != 0)
                    continue;

                if (_gameplayLootService.LootIsBusy)
                    continue;
                
                OrderSetup orderDataSetup = order.OrderData.Setup;
                order.isComplete = true;

                if (_ordersService.OrderPassesConditions())
                {
                    order.isReject = false;
                    GiveRewardForOrder(order, orderDataSetup);
                }
                else
                {
                    order.isReject = true;
                }
            }
        }

        private void GiveRewardForOrder(GameEntity order, OrderSetup orderDataSetup)
        {
            float rewardAmount = _ordersService.GetRewardForOrder().Amount;
            rewardAmount *= _ordersService.GetOrderProgress();
            rewardAmount *= _ordersService.GetPenaltyFactor();
            int rewardRounded = Mathf.RoundToInt(rewardAmount);
            order.AddOrderReward(new CostSetup(orderDataSetup.GoldReward.CurrencyType, rewardRounded));
            _currencyFactory.CreateAddCurrencyRequest(orderDataSetup.GoldReward.CurrencyType, rewardRounded, rewardRounded);
        }
    }
}