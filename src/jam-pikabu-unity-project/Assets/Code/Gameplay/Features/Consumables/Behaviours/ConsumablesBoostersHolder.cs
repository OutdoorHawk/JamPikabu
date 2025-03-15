using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.Features.Days.Service;
using Cysharp.Threading.Tasks;
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
        private readonly Dictionary<ConsumableTypeId, ConsumableBoosterButton> _buttonsDict = new();

        public List<ConsumableBoosterButton> Buttons => _buttons;
        public IReadOnlyDictionary<ConsumableTypeId, ConsumableBoosterButton> ButtonsDict => _buttonsDict;

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
            _daysService.OnDayBegin += InitButtons;
            _consumablesUIService.OnConsumablesUpdated += RefreshButtons;
            _gameStateService.OnStateSwitched += RefreshButtons;
        }

        private void OnDestroy()
        {
            _daysService.OnDayBegin -= InitButtons;
            _consumablesUIService.OnConsumablesUpdated -= RefreshButtons;
            _gameStateService.OnStateSwitched -= RefreshButtons;
        }

        private void InitButtons()
        {
            if (_daysService.BonusLevelType is not BonusLevelType.None)
                return;
            
            for (ConsumableTypeId i = 0; i < ConsumableTypeId.Count; i++)
            {
                if (i is not ConsumableTypeId.None)
                    CreateButton(i);
            }

            RefreshAsync().Forget();
        }

        private void CreateButton(ConsumableTypeId typeId)
        {
            var button = _instantiator.InstantiatePrefabForComponent<ConsumableBoosterButton>(ButtonTemplate, ConsumablesBoostersGrid.transform);
            button.Init(typeId);
            _buttons.Add(button);
            _buttonsDict.Add(typeId, button);
        }

        private void RefreshButtons()
        {
            foreach (ConsumableBoosterButton button in _buttons)
            {
                button.Refresh();
            }
        }

        private async UniTaskVoid RefreshAsync()
        {
            await UniTask.Yield(destroyCancellationToken);
            RefreshButtons();
        }
    }
}