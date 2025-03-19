using Code.Infrastructure.Intro;

namespace Code.Infrastructure.States.GameStates.Bootstrap
{
    public readonly struct BootstrapStatePayload 
    {
        public readonly IntroAnimator IntroAnimator;

        public BootstrapStatePayload(IntroAnimator introAnimator)
        {
            IntroAnimator = introAnimator;
        }
    }
}