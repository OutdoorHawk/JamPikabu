using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Customers.Config;
using Code.Gameplay.Features.Customers.Service;
using Code.Gameplay.Features.RoundState.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Customers.Behaviours
{
    public class CustomerBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _customerImage;
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _playOnAwake;

        private IRoundStateService _roundStateService;
        private ICustomersService _customersService;

        [Inject]
        private void Construct(IRoundStateService roundStateService, ICustomersService customersService)
        {
            _customersService = customersService;
            _roundStateService = roundStateService;
        }

        private void Awake()
        {
            _roundStateService.OnEnterRoundPreparation += UpdateCustomer;
            _roundStateService.OnDayComplete += Hide;

            UpdateSpriteAndShow();
        }

        private void OnDestroy()
        {
            _roundStateService.OnEnterRoundPreparation -= UpdateCustomer;
            _roundStateService.OnDayComplete -= Hide;
        }

        private void UpdateCustomer()
        {
            UpdateAsync().Forget();
        }

        private void Hide()
        {
            _animator.SetTrigger(AnimationParameter.Hide.AsHash());
        }

        private async UniTaskVoid UpdateAsync()
        {
            await _animator.WaitForAnimationCompleteAsync(AnimationParameter.Hide.AsHash(), destroyCancellationToken);
            UpdateSpriteAndShow();
        }

        private void UpdateSpriteAndShow()
        {
            CustomerSetup customer = _customersService.GetCustomerSetup();
            _customerImage.sprite = customer.Sprite;
            if (_playOnAwake)
                _animator.SetTrigger(AnimationParameter.Swap.AsHash());
        }
    }
}