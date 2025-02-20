using System;
using System.Collections.Generic;
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

        private readonly List<ConsumableBoosterButton> _buttons = new();

        [Inject]
        private void Construct
        (
            IDaysService daysService,
            IConsumablesUIService consumablesUIService,
            IInstantiator instantiator
        )
        {
            _instantiator = instantiator;
            _consumablesUIService = consumablesUIService;
            _daysService = daysService;
        }

        private void Awake()
        {
            _daysService.OnDayBegin += InitButtons;
        }

        private void OnDestroy()
        {
            _daysService.OnDayBegin -= InitButtons;
        }

        private void InitButtons()
        {
            IReadOnlyList<PurchasedConsumableData> purchased = _consumablesUIService.GetActiveConsumables();
            
            foreach (PurchasedConsumableData data in purchased)
            {
                switch (data.Type)
                {
                    case ConsumableTypeId.None:
                        break;
                    case ConsumableTypeId.Wood:
                        CreateButton(in data);
                        break;
                    case ConsumableTypeId.Spoon:
                        CreateButton(in data);
                        break;
                }
            }
        }

        private void CreateButton(in PurchasedConsumableData data)
        {
            var button = _instantiator.InstantiatePrefabForComponent<ConsumableBoosterButton>(ButtonTemplate, ConsumablesBoostersGrid.transform);
            button.Init(in data);
        }
    }
}