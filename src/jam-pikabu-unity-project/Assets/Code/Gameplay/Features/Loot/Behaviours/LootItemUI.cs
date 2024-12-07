using Code.Common;
using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
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
        public IconablePrice Price;
        public Image Icon;
        public CanvasGroup CanvasGroup;
        public Animator LootAnimator;
        public float FlyToVatDuration = 1f;

        private Tweener _lootItemTween;

        private int _currentValue;
        private int _currentWithdrawValue;

        public LootTypeId Type { get; private set; }

        public void Init(LootSetup setup)
        {
            Icon.sprite = setup.Icon;
            Type = setup.Type;
        }

        public void InitPrice(CostSetup rating)
        {
            Price.SetupPrice(rating);
            Price.EnableElement();
        }

        public async UniTask AnimateEffectProducer()
        {
            LootAnimator.SetTrigger(AnimationParameter.EffectProducer.AsHash());
            await DelaySeconds(1, destroyCancellationToken);
        }

        public async UniTask AnimateEffectTarget()
        {
            await DelaySeconds(0.25f, destroyCancellationToken);
            LootAnimator.WaitForAnimationCompleteAsync(AnimationParameter.EffectTarget.AsHash(), destroyCancellationToken).Forget();
            await DelaySeconds(0.25f, destroyCancellationToken);
        }

        public async UniTask AnimateFlyToVat(Transform flyEndPoint)
        {
            await transform
                    .DOJump(flyEndPoint.position, Random.Range(1, 4), 1, FlyToVatDuration)
                    .SetLink(gameObject)
                    .AsyncWaitForCompletion()
                ;
        }

        public async UniTask AnimateConsume()
        {
            await LootAnimator.WaitForAnimationCompleteAsync(AnimationParameter.Consume.AsHash(), destroyCancellationToken);
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

        public void AddGoldValueWithdraw(int withdraw)
        {
            _currentWithdrawValue += withdraw;
        }

        private void SetReadyToApply()
        {
            _lootItemTween?.Kill();
            _lootItemTween = null;

            if (Entity.IsNullOrDestructed())
                return;

            Entity.isBusy = false;
        }
    }
}