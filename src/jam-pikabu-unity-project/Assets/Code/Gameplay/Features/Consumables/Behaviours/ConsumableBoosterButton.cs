using Code.Gameplay.Features.Consumables.Factory;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Consumables.Data;
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
        public Image Icon;
        public Button Button;
        public TMP_Text Amount;

        private IStaticDataService _staticDataService;
        private IConsumablesUIService _consumablesUIService;
        private IConsumablesFactory _consumablesFactory;
        private IGameStateService _gameStateService;

        public ConsumableTypeId Type { get; private set; }

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
            _gameStateService.OnStateSwitched += Refresh;
            _consumablesUIService.OnConsumablesUpdated += Refresh;
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnButtonClicked);
            _gameStateService.OnStateSwitched += Refresh;
            _consumablesUIService.OnConsumablesUpdated -= Refresh;
        }

        public void Init(in PurchasedConsumableData data)
        {
            Type = data.Type;
            ShopItemData shopData = _staticDataService
                .Get<ShopStaticData>()
                .GetByConsumableType(data.Type);

            Icon.sprite = shopData.Icon;

            Refresh();
        }

        private void Refresh()
        {
            RefreshButton();
            RefreshText();
        }

        private void RefreshText()
        {
            Amount.text = $"x {_consumablesUIService.GetConsumableAmount(Type)}";
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

            Button.interactable = true;
        }

        private void OnButtonClicked()
        {
            _consumablesFactory.ActivateConsumable(Type);
        }
    }
}