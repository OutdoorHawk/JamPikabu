using System.Collections;
using Code.Common.Extensions;
using Code.Gameplay.Features.Consumables.Config;
using Code.Gameplay.Features.Consumables.Factory;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Ads.Behaviours;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.UI.Shop.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Consumables.Behaviours
{
    public class ConsumableBoosterButton : MonoBehaviour
    {
        public Image IconBack;
        public Image IconFilled;
        public AdsButton ButtonAds;
        public GameObject AdsIcon;
        public Button Button;
        public TMP_Text Amount;
        public TMP_Text AdsAmount;

        private IStaticDataService _staticDataService;
        private IConsumablesUIService _consumablesUIService;
        private IConsumablesFactory _consumablesFactory;
        private IGameStateService _gameStateService;

        public ConsumableTypeId Type { get; private set; }
        private ConsumablesStaticData StaticData => _staticDataService.Get<ConsumablesStaticData>();

        private Coroutine _cooldownRoutine;

        [Inject]
        private void Construct
        (
            IStaticDataService staticDataService,
            IConsumablesUIService consumablesUIService,
            IConsumablesFactory consumablesFactory,
            IGameStateService gameStateService
        )
        {
            _gameStateService = gameStateService;
            _consumablesFactory = consumablesFactory;
            _consumablesUIService = consumablesUIService;
            _staticDataService = staticDataService;
        }

        private void Awake()
        {
            Button.onClick.AddListener(OnButtonClicked);
            ButtonAds.OnRewarded += AdRewarded;
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnButtonClicked);
            ButtonAds.OnRewarded -= AdRewarded;
        }

        private void OnDisable()
        {
            _cooldownRoutine = null;
        }

        public void Init(ConsumableTypeId typeId)
        {
            Type = typeId;

            ShopItemData shopData = _staticDataService
                .Get<ShopStaticData>()
                .GetByConsumableType(typeId);

            IconBack.sprite = shopData.Icon;
            IconBack.color = Color.white;
            AdsAmount.text = $"+{StaticData.GetConsumableData(typeId).AdRewardedAmount}";
        }

        public void Refresh()
        {
            RefreshState();
            RefreshButton();
            RefreshAdsButton();
            RefreshText();
        }

        private void RefreshText()
        {
            int amount = _consumablesUIService.GetConsumableAmount(Type);

            Amount.text = amount == 0
                ? string.Empty
                : $"x {amount.ToString()}";
        }

        private void RefreshState()
        {
            if (_consumablesUIService.ConsumableUnlocked(Type) == false)
            {
                gameObject.DisableElement();
                return;
            }

            gameObject.EnableElement();
        }

        private void RefreshButton()
        {
            if (_cooldownRoutine != null)
            {
                Button.interactable = false;
                return;
            }

            if (_consumablesUIService.GetConsumableAmount(Type) <= 0)
            {
                Button.interactable = false;
                return;
            }

            if (_gameStateService.CurrentState is not GameStateTypeId.RoundLoop)
            {
                Button.interactable = false;
                return;
            }

            if (_cooldownRoutine != null)
            {
                Button.interactable = false;
                return;
            }

            Button.interactable = true;
        }

        private void RefreshAdsButton()
        {
            ButtonAds.Button.interactable = _cooldownRoutine == null;

            if (ButtonAds.RewardedAdsAvailable == false)
            {
                AdsIcon.DisableElement();
                ButtonAds.DisableElement();
                AdsAmount.DisableElement();
                return;
            }

            if (_consumablesUIService.GetConsumableAmount(Type) <= 0)
            {
                AdsIcon.EnableElement();
                ButtonAds.EnableElement();
                AdsAmount.EnableElement();
                return;
            }

            AdsIcon.DisableElement();
            ButtonAds.DisableElement();
            AdsAmount.DisableElement();
        }

        private void OnButtonClicked()
        {
            _consumablesFactory.ActivateConsumable(Type);
            StartCooldown();
            RefreshButton();
        }

        private void AdRewarded()
        {
            for (int i = 0; i < StaticData.GetConsumableData(Type).AdRewardedAmount; i++)
                _consumablesUIService.AddConsumable(Type);
            
            Refresh();
        }

        private void StartCooldown()
        {
            ConsumablesData data = StaticData.GetConsumableData(Type);
            _cooldownRoutine = StartCoroutine(CooldownRoutine(data.CooldownSeconds));
            IconFilled.fillAmount = 1;
        }

        private IEnumerator CooldownRoutine(float cooldownSeconds)
        {
            while (_gameStateService.CurrentState is GameStateTypeId.RoundLoop)
                yield return null;

            _cooldownRoutine = null;
            Refresh();
            IconFilled.fillAmount = 0;
        }
    }
}