using System.Threading;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Infrastructure.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static System.Threading.CancellationTokenSource;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.GrapplingHook.Behaviours
{
    public class GrapplingHookBehaviour : EntityDependant
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _moveAngleDuration = 0.25f;
        [SerializeField] private float _ascentDelay = 0.25f;
        [SerializeField] private float _openClawsDelay = 0.2f;
        [SerializeField] private float _collectAnimationDelay = 1f;

        private ISoundService _soundService;
        private CancellationTokenSource _movementBlendSource;
        private CancellationTokenSource _speedModificationSource;
        private float _lastMoveDirection;

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        public void SetupXMovement(float moveDirection)
        {
            if (Mathf.Approximately(_lastMoveDirection, moveDirection))
                return;

            _movementBlendSource?.Cancel();
            _movementBlendSource = CreateLinkedTokenSource(destroyCancellationToken);

            _animator.ToFloatParameter
            (
                AnimationParameter.MovementBlend.AsHash(),
                moveDirection,
                _moveAngleDuration,
                _movementBlendSource.Token
            ).Forget();

            _lastMoveDirection = moveDirection;
        }

        public void OpenClaws()
        {
            _animator.SetBool(AnimationParameter.Open.AsHash(), true);
        }

        public void CloseClawsAndReturn()
        {
            CloseAndAscentAsync().Forget();
        }

        public void StartDescending()
        {
            if (Entity.isDescending)
                return;

            if (Entity.isClosingClaws)
                return;

            _soundService.PlayOneShotSound(SoundTypeId.HookDescend);
        }

        public void ApplySpeedChange(float factor, float duration)
        {
            ApplySpeedChangeAsync(factor, duration).Forget();
        }

        private async UniTaskVoid CloseAndAscentAsync()
        {
            Entity.isClosingClaws = true;

            _animator.SetBool(AnimationParameter.Open.AsHash(), false);

            await DelaySeconds(_ascentDelay, destroyCancellationToken);

            if (Entity != null)
                Entity.isAscentRequested = true;

            if (Entity != null)
                Entity.isClosingClaws = false;
        }

        private async UniTaskVoid ApplySpeedChangeAsync(float factor, float duration)
        {
            _speedModificationSource?.Cancel();
            _speedModificationSource = CreateLinkedTokenSource(destroyCancellationToken);
            
            bool flip = Random.value < 0.5f;

            if (flip)
                factor *= -1;

            Entity.ReplaceHookSpeedModifier(1 + factor);
            
            await DelaySeconds(duration, _speedModificationSource.Token);
            
            Entity.ReplaceHookSpeedModifier(1);
        }
    }
}