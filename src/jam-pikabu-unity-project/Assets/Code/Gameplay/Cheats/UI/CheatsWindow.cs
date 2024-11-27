using System.Collections.Generic;
using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Gameplay.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Cheats.UI
{
    public class CheatsWindow : BaseWindow
    {
        [SerializeField] private VerticalLayoutGroup _cheatsLayout;
        [SerializeField] private CheatActionButton _baseCheatButton;
        [SerializeField] private CheatActionButtonWithInputField _cheatButtonWithInputField;

        private List<ICheatAction> _cheatActions;

        [Inject]
        private void Construct
        (
            List<ICheatAction> cheatActions
        )
        {
            _cheatActions = cheatActions;
        }

        protected override void Initialize()
        {
            base.Initialize();

            ShowCheats();
            CloseButton.transform.SetAsLastSibling();
        }

        private void ShowCheats()
        {
            _cheatActions.Sort((x, y) => x.Order.CompareTo(y.Order));
            
            foreach (var cheatAction in _cheatActions) 
                InitCheatActionButton(cheatAction);
        }

        private void InitCheatActionButton(ICheatAction cheatAction)
        {
            switch (cheatAction)
            {
                case ICheatActionInputString cheatActionInputString:
                {
                    var cheatButton = Instantiate(_cheatButtonWithInputField, _cheatsLayout.transform);
                    cheatButton.CheatAction = input => cheatActionInputString.Execute(input);
                    cheatButton.ButtonText.text = cheatActionInputString.CheatLabel;
                    break;
                }
                case ICheatActionBasic cheatActionBasic:
                {
                    var cheatButton = Instantiate(_baseCheatButton, _cheatsLayout.transform);
                    cheatButton.CheatAction = () => cheatActionBasic.Execute();
                    cheatButton.ButtonText.text = cheatActionBasic.CheatLabel;
                    break;
                }
            }
        }
    }
}