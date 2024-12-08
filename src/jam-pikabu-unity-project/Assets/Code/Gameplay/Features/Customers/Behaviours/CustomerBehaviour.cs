using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Customers.Config;
using Code.Gameplay.Features.Customers.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Customers.Behaviours
{
    public class CustomerBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _customerImage;
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _hideOnAwake;
        [SerializeField] private float _openOrderWindowDelay = 0.2f;

        private IRoundStateService _roundStateService;
        private ICustomersService _customersService;
        private IOrdersService _ordersService;
        private IWindowService _windowService;
        private ISoundService _soundService;

        private bool _hided;

        [Inject]
        private void Construct(IRoundStateService roundStateService,
            ICustomersService customersService,
            IOrdersService ordersService,
            IWindowService windowService,
            ISoundService soundService)
        {
            _soundService = soundService;
            _windowService = windowService;
            _ordersService = ordersService;
            _customersService = customersService;
            _roundStateService = roundStateService;
        }

        private void Awake()
        {
            _ordersService.OnOrderUpdated += UpdateCustomer;
            _roundStateService.OnDayComplete += Hide;

            //UpdateSprite();

            if (_hideOnAwake)
                _hided = true;
        }

        private void OnDestroy()
        {
            _ordersService.OnOrderUpdated -= UpdateCustomer;
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
            if (_hided == false)
            {
                await _animator.WaitForAnimationCompleteAsync(AnimationParameter.Hide.AsHash(), destroyCancellationToken);
                _soundService.PlayOneShotSound(SoundTypeId.CustomerSwap);
            }
            
            _hided = false;
            UpdateSprite();
            _soundService.PlayOneShotSound(SoundTypeId.CustomerSwap);
            await _animator.WaitForAnimationCompleteAsync(AnimationParameter.Show.AsHash(), destroyCancellationToken);
            await DelaySeconds(_openOrderWindowDelay, destroyCancellationToken);
            _windowService.OpenWindow(WindowTypeId.OrderWindow);
        }

        private void UpdateSprite()
        {
            CustomerSetup customer = _customersService.GetCustomerSetup();
            _customerImage.sprite = customer.Sprite;
        }
    }
}