using System;
using System.Collections;
using Code.Common.Extensions;
using Code.Gameplay.Features.Consumables.Config;
using Code.Gameplay.Features.Consumables.Factory;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.UI.Shop.Configs;
using DG.Tweening;
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
        public Button Button;
        public TMP_Text Amount;

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
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnButtonClicked);
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

            Refresh();
        }

        public void Refresh()
        {
            RefreshState();
            RefreshButton();
            RefreshText();
        }

        private void RefreshText()
        {
            Amount.text = $"x {_consumablesUIService.GetConsumableAmount(Type)}";
        }

        private void RefreshState()
        {
            if (_consumablesUIService.ConsumableExist(Type) == false)
            {
                gameObject.DisableElement();
                return;
            }

            gameObject.EnableElement();
        }

        private void RefreshButton()
        {
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

        private void OnButtonClicked()
        {
            _consumablesFactory.ActivateConsumable(Type);
            StartCooldown();
        }

        private void StartCooldown()
        {
            ConsumablesData data = StaticData.GetConsumableData(Type);
            _cooldownRoutine = StartCoroutine(CooldownRoutine(data.CooldownSeconds));
            IconFilled.fillAmount = 1;
            IconFilled.DOFillAmount(0, data.CooldownSeconds)
                .SetEase(Ease.Linear)
                .SetLink(gameObject);
        }

        private IEnumerator CooldownRoutine(float cooldownSeconds)
        {
            yield return new WaitForSeconds(cooldownSeconds);
            _cooldownRoutine = null;
            Refresh();
        }
    }
}