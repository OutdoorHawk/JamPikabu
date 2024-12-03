using Code.Common;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Loot.Configs;
using Code.Infrastructure.View;
using Code.Meta.UI.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Loot.Behaviours
{
    public class LootItemUI : EntityDependant
    {
        public PriceInfo Value;
        public Image Icon;
        public CanvasGroup CanvasGroup;
        public Animator LootAnimator;

        private Tweener _lootItemTween;
        
        private int _currentValue;
        private int _currentWithdrawValue;

        public void Init(LootSetup setup)
        {
            Icon.sprite = setup.Icon;
            Value.SetupPrice(setup.Value);
            CanvasGroup.alpha = 0;
            _currentValue = setup.Value.Amount;
        }

        public void UpdateValue(int newValue)
        {
            if (_currentValue == newValue)
                return;
            
            _currentValue = newValue;
            UpdateView();
        }

        public async UniTask AnimateEffectProducer()
        {
            Entity.isBusy = true;
            LootAnimator.SetTrigger(AnimationParameter.EffectProducer.AsHash());
            await DelaySeconds(1, destroyCancellationToken);
            Entity.isBusy = false;
        }

        public async UniTask AnimateEffectTarget()
        {
            Entity.isBusy = true;
            await DelaySeconds(1, destroyCancellationToken);
            LootAnimator.SetTrigger(AnimationParameter.EffectTarget.AsHash());
            Entity.isBusy = false;
        }

        public async UniTask AnimateConsume()
        {
            Entity.isBusy = true;
            await DelaySeconds(0.5f, destroyCancellationToken);
            Entity.isBusy = false;
        }

        public void Show()
        {
            Entity.isBusy = true;
            _lootItemTween?.Kill();
            _lootItemTween = CanvasGroup
                    .DOFade(1, 0.15f)
                    .SetLink(gameObject)
                    .OnComplete(SetReadyToApply)
                ;
        }

        public void SetGoldValueWithdraw(int withdraw)
        {
            _currentWithdrawValue = withdraw;
            UpdateView();
        }

        private void UpdateView()
        {
            Value.SetupPrice(_currentValue + _currentWithdrawValue, CurrencyTypeId.Gold);
        }

        private void SetReadyToApply()
        {
            _lootItemTween?.Kill();
            _lootItemTween = null;

            if (Entity.IsNullOrDestructed())
                return;

            Entity.isReadyToApply = true;
            Entity.isBusy = false;
        }
    }
}