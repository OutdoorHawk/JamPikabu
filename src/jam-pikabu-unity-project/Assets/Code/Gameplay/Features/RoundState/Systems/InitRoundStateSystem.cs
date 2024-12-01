using Code.Gameplay.Features.RoundState.Factory;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class InitRoundStateSystem : IInitializeSystem
    {
        private readonly IRoundStateFactory _roundStateFactory;

        public InitRoundStateSystem(GameContext context, IRoundStateFactory roundStateFactory)
        {
            _roundStateFactory = roundStateFactory;
            
            context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.RoundStateController));
        }

        public void Initialize()
        {
            _roundStateFactory.CreateRoundStateController();
        }
    }
}