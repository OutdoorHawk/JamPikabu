using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Entitas;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class SetDayCheat : BaseCheat, ICheatActionInputString
    {
        public string CheatLabel => "Выставить день";
        public OrderType Order => OrderType.Second;
        
        public void Execute(string input)
        {
            IGroup<MetaEntity> days = _metaContext.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.Day));
            
            foreach (var day in days)
            {
                day.ReplaceDay(int.Parse(input));
            }
        }
    }
}