using System;
using Code.Gameplay.Input.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Code.Infrastructure.Intro
{
    public class IntroAnimator : MonoBehaviour
    {
        public IntroVideoPlayer VideoPlayer;

        private UniTaskCompletionSource _animationFinishedSource;
        
        private IInputService _inputService;

        [Inject]
        private void Construct
        (
            IInputService inputService
        )
        {
            _inputService = inputService;
        }

        private void Awake()
        {
            InitSource();
        }

        private void Start()
        {
            PlayIntroAnimation().Forget();
            _inputService.PlayerInput.Player.Jump.performed += SkipIntro;
        }

        private void OnDestroy()
        {
            _inputService.PlayerInput.Player.Jump.performed -= SkipIntro;
            _animationFinishedSource?.TrySetResult();
        }

        private void SkipIntro(InputAction.CallbackContext _)
        {
            _animationFinishedSource?.TrySetResult();
        }

        public async UniTask WaitForAnimationCompleteAsync()
        {
            await _animationFinishedSource.Task;
            await VideoPlayer.HidePlayer();

            Destroy(gameObject);
        }

        private void InitSource()
        {
            _animationFinishedSource = new UniTaskCompletionSource();
        }

        private async UniTaskVoid PlayIntroAnimation()
        {
            await VideoPlayer.WaitForVideoCompleteAsync();
            _animationFinishedSource.TrySetResult();
        }
    }
}