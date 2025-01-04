using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Behaviours
{
    public class AbilityVisuals : MonoBehaviour
    {
        public Rigidbody2D Rigidbody;
        public float SwapDuration = 0.25f;

        public void PlaySwap(Vector3 newPosition)
        {
            PlaySwapAsync(newPosition).Forget();
        }

        private async UniTaskVoid PlaySwapAsync(Vector3 newPosition)
        {
            await transform
                    .DOScale(1.2f, SwapDuration / 3)
                    .SetLink(gameObject)
                    .AsyncWaitForCompletion()
                ;

            await transform
                    .DOScale(0, SwapDuration)
                    .SetLink(gameObject)
                    .AsyncWaitForCompletion()
                ;

            if (Rigidbody == null)
                return;

            Rigidbody.position = newPosition;

            await transform
                    .DOScale(1, SwapDuration)
                    .SetLink(gameObject)
                    .SetEase(Ease.OutBounce)
                    .AsyncWaitForCompletion()
                ;
        }
    }
}