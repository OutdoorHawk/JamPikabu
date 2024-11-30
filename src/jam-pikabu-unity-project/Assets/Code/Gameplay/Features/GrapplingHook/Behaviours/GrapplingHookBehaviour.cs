using System.Threading;
using Code.Common.Extensions.Animations;
using Code.Infrastructure.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
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

        private CancellationTokenSource _movementBlendSource;
        private float _lastMoveDirection;

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
            WaitForLootAndOpenAsync().Forget();
        }

        public void CloseClawsAndReturn()
        {
            CloseAndAscentAsync().Forget();
        }

        private async UniTaskVoid WaitForLootAndOpenAsync()
        {
            await DelaySeconds(_openClawsDelay, destroyCancellationToken);
            _animator.SetBool(AnimationParameter.Open.AsHash(), true);
            
            await DelaySeconds(_collectAnimationDelay, destroyCancellationToken);
            
            Entity.isCollectLootRequest = false; //todo temporary
        }

        private async UniTaskVoid CloseAndAscentAsync()
        {
            _animator.SetBool(AnimationParameter.Open.AsHash(), false);

            await DelaySeconds(_ascentDelay, destroyCancellationToken);

            if (Entity != null)
            {
                Entity.isAscentRequested = true;
                Entity.isDescentAvailable = false;
                Entity.isDescending = false;
            }
        }
    }
}