using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.Features.Days.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Consumables.Behaviours
{
    public class ConsumablesBoostersHolder : MonoBehaviour
    {
        public GridLayoutGroup ConsumablesBoostersGrid;
        public ConsumableBoosterButton ButtonTemplate;

        private IDaysService _daysService;
        private IInstantiator _instantiator;
        private IConsumablesUIService _consumablesUIService;
        private IGameStateService _gameStateService;

        private readonly List<ConsumableBoosterButton> _buttons = new();

        [Inject]
        private void Construct
        (
            IDaysService daysService,
            IConsumablesUIService consumablesUIService,
            IInstantiator instantiator,
            IGameStateService gameStateService
        )
        {
            _gameStateService = gameStateService;
            _instantiator = instantiator;
            _consumablesUIService = consumablesUIService;
            _daysService = daysService;
        }

        private void Awake()
        {
            _daysService.OnDayBegin += RefreshButtons;
            _consumablesUIService.OnConsumablesUpdated += RefreshButtons;
            _gameStateService.OnStateSwitched += RefreshButtons;
        }

        private void OnDestroy()
        {
            _daysService.OnDayBegin -= RefreshButtons;
            _consumablesUIService.OnConsumablesUpdated -= RefreshButtons;
            _gameStateService.OnStateSwitched -= RefreshButtons;
        }

        private void RefreshButtons()
        {
            IReadOnlyList<PurchasedConsumableData> purchased = _consumablesUIService.GetActiveConsumables();

            foreach (PurchasedConsumableData data in purchased)
            {
                switch (data.Type)
                {
                    case ConsumableTypeId.None:
                        break;
                    case ConsumableTypeId.Wood:
                        UpdateButton(in data);
                        break;
                    case ConsumableTypeId.Spoon:
                        UpdateButton(in data);
                        break;
                }
            }
        }
        
        private void UpdateButton(in PurchasedConsumableData data)
        {
            ConsumableTypeId type = data.Type;
            ConsumableBoosterButton button = _buttons.Find(boosterButton => boosterButton.Type == type);

            if (button != null)
            {
                button.Init(data);
                return;
            }

            button = _instantiator.InstantiatePrefabForComponent<ConsumableBoosterButton>(ButtonTemplate, ConsumablesBoostersGrid.transform);
            button.Init(in data);
            _buttons.Add(button);
        }
    }
}