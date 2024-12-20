using System;
using Code.Common;
using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Orders;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Infrastructure.View;
using Code.Meta.UI.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Features.Loot.Behaviours
{
    public class LootItemUI : MonoBehaviour
    {
        public IconablePrice Price;
        public Image Icon;
        public Image IconBackground;
        public CanvasGroup CanvasGroup;
        public Animator LootAnimator;
        public TMP_Text AmountNeedText;
        public Color GoodBackColor;
        public Color BadBackColor;
        public float FlyToVatDuration = 1f;

        private ISoundService _soundService;
        private Tweener _lootItemTween;
        private IngredientTypeId _ingredientTypeId;

        public int AmountNeed { get; private set; }
        public bool CollectedAtLeastOne { get; private set; }
        public LootTypeId Type { get; private set; }

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        private void Awake()
        {
            CanvasGroup.alpha = 0;
        }

        public void InitType(LootSetup setup)
        {
            Icon.sprite = setup.Icon;
            Type = setup.Type;
        }

        public void InitItem(in IngredientData ingredientData)
        {
            _ingredientTypeId = ingredientData.IngredientType;
            AmountNeed = ingredientData.Amount;
            UpdateNeedAmount();
            
            if (_ingredientTypeId is IngredientTypeId.Good)
                IconBackground.color = GoodBackColor;
            if (_ingredientTypeId is IngredientTypeId.Bad) 
                IconBackground.color = BadBackColor;
        }

        public void InitRatingFactor(CostSetup rating)
        {
            Price.SetupPrice(rating);
            Price.EnableElement();
        }

        public async UniTask AnimateFlyToVat(Transform flyEndPoint)
        {
            await LootAnimator.WaitForAnimationCompleteAsync(AnimationParameter.Fly.AsHash(), destroyCancellationToken);
            
            _soundService.PlaySound(SoundTypeId.Construction_Fly);
            await transform
                    .DOJump(flyEndPoint.position, Random.Range(1, 4), 1, FlyToVatDuration)
                    .SetLink(gameObject)
                    .AsyncWaitForCompletion()
                ;
        }

        public void AnimateCollected()
        {
            if (this == null)
                return;
            
            if (LootAnimator == null)
                return;
            
            CollectedAtLeastOne = true;

            if (_ingredientTypeId is IngredientTypeId.Bad)
            {
                LootAnimator.SetTrigger(AnimationParameter.Replenish.AsHash());
                return;
            }
            
            if (AmountNeed == 1)
            {
                AnimateComplete();
                return;
            }

            if (AmountNeed == 0)
            {
                LootAnimator.SetTrigger(AnimationParameter.Replenish.AsHash());
                return;
            }
            
            AmountNeed--;
            UpdateNeedAmount();
            LootAnimator.SetTrigger(AnimationParameter.Replenish.AsHash());
        }

        public async UniTask AnimateConsume()
        {
            await LootAnimator.WaitForAnimationCompleteAsync(AnimationParameter.Consume.AsHash(), destroyCancellationToken);
        }

        private void AnimateComplete()
        {
            AmountNeed = 0;
            LootAnimator.SetTrigger(AnimationParameter.Complete.AsHash());
        }

        public void Show()
        {
            CanvasGroup.alpha = 0;
            _lootItemTween?.Kill();
            _lootItemTween = CanvasGroup
                    .DOFade(1, 0.15f)
                    .SetLink(gameObject)
                    .OnComplete(SetReadyToApply)
                ;

            _soundService.PlaySound(SoundTypeId.Construction_Place);
        }

        private void UpdateNeedAmount()
        {
            AmountNeedText.text = "";
            
            if (_ingredientTypeId == IngredientTypeId.Good)
            {
                AmountNeedText.text = $"x{AmountNeed}";
            }
        }

        private void SetReadyToApply()
        {
            _lootItemTween?.Kill();
            _lootItemTween = null;
        }
    }
}