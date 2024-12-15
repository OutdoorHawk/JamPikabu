using System.Collections.Generic;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.Windows.Factory;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;
using Entitas;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class PlayGoldForOrderVisualsSystem : ReactiveSystem<GameEntity>
    {
        private readonly IWindowService _windowService;
        private readonly IUIFactory _uiFactory;
        private readonly ILootService _lootService;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ISoundService _soundService;

        private readonly List<UniTask> _tasksBuffer = new();

        public PlayGoldForOrderVisualsSystem(GameContext context,
            IWindowService windowService,
            IUIFactory uiFactory,
            ILootService lootService,
            ICurrencyFactory currencyFactory,
            ISoundService soundService
        ) :
            base(context)
        {
            _windowService = windowService;
            _uiFactory = uiFactory;
            _lootService = lootService;
            _currencyFactory = currencyFactory;
            _soundService = soundService;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher
                .AllOf(GameMatcher.Order)
                .AnyOf(GameMatcher.Complete)
                .Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var order in entities)
            {
                PlayOrderCompleteAnimation(order).Forget();
            }
        }

        private async UniTask PlayOrderCompleteAnimation(GameEntity order)
        {
            if (_lootService.CollectedLootItems.Count == 0)
            {
                order.isResultProcessed = true;
                return;
            }

            if (order.hasOrderReward == false)
            {
                order.isResultProcessed = true;
                return;
            }

            _uiFactory.SetRaycastAvailable(false);
            _windowService.TryGetWindow(out PlayerHUDWindow hud);

            CostSetup orderReward = order.OrderReward;
            var parameters = new CurrencyAnimationParameters
            {
                Type = orderReward.CurrencyType,
                Count = orderReward.Amount,
                TextPrefix = "+",
                StartReplenishSound = SoundTypeId.Gold_Currency_Collect,
                StartPosition = hud.OrderViewBehaviour.Reward.CurrencyIcon.transform.position,
                EndPosition = hud.CurrencyHolder.PlayerCurrentGold.CurrencyIcon.transform.position,
                StartReplenishCallback = () => _currencyFactory.CreateAddCurrencyRequest(orderReward.CurrencyType, 0, -orderReward.Amount)
            };

            const float delayToFinishOtherAnimations = 0.75f;
            const float goldAnimationDelay = 0.75f;
            await DelaySeconds(delayToFinishOtherAnimations, hud.destroyCancellationToken);
            _currencyFactory.PlayCurrencyAnimation(parameters);
            await DelaySeconds(goldAnimationDelay, hud.destroyCancellationToken);
            _soundService.PlaySound(SoundTypeId.Order_Completed);
            _uiFactory.SetRaycastAvailable(true);
            order.isResultProcessed = true;
        }
    }
}