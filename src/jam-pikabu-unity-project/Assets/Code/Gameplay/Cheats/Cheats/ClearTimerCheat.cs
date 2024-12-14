using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Entitas;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class ClearTimerCheat : BaseCheat, ICheatActionBasic
    {
        public string CheatLabel => "Обнулить таймер раунда";
        public OrderType Order => OrderType.Second;

        public void Execute()
        {
            IGroup<GameEntity> timers = _gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.RoundInProcess,
                GameMatcher.RoundTimeLeft));
            
            foreach (var timer in timers)
            {
                timer.ReplaceRoundTimeLeft(0);
            }
        }
    }
}