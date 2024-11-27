using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Windows.Common
{
    public class InfoWindow : BaseWindow
    {
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        private Action _leftButtonAction;
        private Action _rightButtonAction;

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();
            _rightButton.onClick.AddListener(RightAction);
            _leftButton.onClick.AddListener(LeftAction);
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            _rightButton.onClick.RemoveListener(RightAction);
            _leftButton.onClick.RemoveListener(LeftAction);
        }

        public void SetActions(Action leftButtonAction, Action rightButtonAction)
        {
            _rightButtonAction = rightButtonAction;
            _leftButtonAction = leftButtonAction;
        }

        private void RightAction()
        {
            _rightButtonAction?.Invoke();
        }

        private void LeftAction()
        {
            _leftButtonAction?.Invoke();
        }
    }
}