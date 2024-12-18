using Code.Common.Extensions.Animations;
using UnityEngine;

namespace Code.Meta.UI.Common.Replenish
{
    public class ReplenishAnimator : MonoBehaviour, IReplenishAnimator
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Replenish()
        {
            animator.SetTrigger(AnimationParameter.Replenish.AsHash());
        }
    }
}