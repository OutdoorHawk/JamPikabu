using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class InitRoundStateSystem : IInitializeSystem
    {
        private readonly IRoundStateService _roundStateService;

        public InitRoundStateSystem(IRoundStateService roundStateService)
        {
            _roundStateService = roundStateService;
        }

        public void Initialize()
        {
            _roundStateService.BeginDay();
        }
    }
}