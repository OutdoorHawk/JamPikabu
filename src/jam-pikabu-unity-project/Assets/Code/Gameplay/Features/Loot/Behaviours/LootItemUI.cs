using Code.Common;
using Code.Common.Extensions.Animations;
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
        public PriceInfo GoldForPickup;
        public Image Icon;
        public CanvasGroup CanvasGroup;
        public Animator LootAnimator;

        private Tweener _lootItemTween;

        public void Init(LootSetup setup)
        {
            Icon.sprite = setup.Icon;
            GoldForPickup.SetupPrice(setup.GoldForPicking);
            CanvasGroup.alpha = 0;
        }

        public async UniTask AnimateEffectProducer()
        {
            LootAnimator.SetTrigger(AnimationParameter.EffectProducer.AsHash());
            await DelaySeconds(1, destroyCancellationToken);
        }

        public void AnimateEffectTarget()
        {
            LootAnimator.SetTrigger(AnimationParameter.EffectTarget.AsHash());
        }

        public void Show()
        {
            _lootItemTween?.Kill();
            _lootItemTween = CanvasGroup
                    .DOFade(1, 0.15f)
                    .SetLink(gameObject)
                    .OnComplete(SetReadyToApply)
                ;
        }

        private void SetReadyToApply()
        {
            _lootItemTween?.Kill();
            _lootItemTween = null;

            if (Entity.IsNullOrDestructed())
                return;

            Entity.isReadyToApply = true;
        }
    }
}